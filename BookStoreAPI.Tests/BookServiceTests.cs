using Xunit;
using Microsoft.EntityFrameworkCore;
using BookStoreAPI.Data;
using BookStoreAPI.Services;
using BookStoreAPI.Models;
using BookStoreAPI.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class BookServiceTests
{
    private readonly BookService _bookService;
    private readonly ApplicationDbContext _context;

    public BookServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // 👈 Unique name per test
            .Options;

        _context = new ApplicationDbContext(options);
        _bookService = new BookService(_context);
    }


    [Fact]
    public async Task GetAll_ShouldReturnAllBooks()
    {
        // Arrange
        _context.Books.AddRange(
            new Book { Name = "Book A", Category = "Tech", Price = 25 },
            new Book { Name = "Book B", Category = "Sci-Fi", Price = 18 }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _bookService.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task Create_ShouldAddBookToDatabase()
    {
        // Arrange
        var bookDto = new BookDto
        {
            Name = "New Book",
            Category = "Testing",
            Price = 19.99m
        };

        // Act
        await _bookService.CreateAsync(bookDto);

        // Assert
        var books = await _context.Books.ToListAsync();

        Assert.Single(books);
        Assert.Equal("New Book", books[0].Name);
        Assert.Equal("Testing", books[0].Category);
        Assert.Equal(19.99m, books[0].Price);
    }

    [Fact]
    public async Task GetById_ShouldReturnCorrectBook()
    {
        // Arrange
        var book = new Book
        {
            Name = "Specific Book",
            Category = "Science",
            Price = 50.0m,
            Description = "ID lookup test"
        };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bookService.GetByIdAsync(book.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(book.Id, result.Id);
        Assert.Equal("Specific Book", result.Name);
        Assert.Equal("Science", result.Category);
        Assert.Equal(50.0m, result.Price);
    }

    [Fact]
    public async Task Update_ShouldModifyBook()
    {
        // Arrange
        var book = new Book
        {
            Name = "Original Book",
            Category = "Old Category",
            Price = 10.0m,
            Description = "Old description"
        };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        var updatedDto = new BookDto
        {
            Name = "Updated Book",
            Category = "New Category",
            Price = 20.0m,
            Description = "Updated description"
        };

        // Act
        var result = await _bookService.UpdateAsync(book.Id, updatedDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Book", result!.Name);
        Assert.Equal("New Category", result.Category);
        Assert.Equal(20.0m, result.Price);
        Assert.Equal("Updated description", result.Description);
    }

    [Fact]
    public async Task Delete_ShouldRemoveBook()
    {
        // Arrange
        var book = new Book
        {
            Name = "Book To Delete",
            Category = "Temp",
            Price = 9.99m,
            Description = "Will be deleted"
        };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bookService.DeleteAsync(book.Id);

        // Assert
        var deleted = await _context.Books.FindAsync(book.Id);
        Assert.True(result);
        Assert.Null(deleted);
    }


}
