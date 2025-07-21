using BookStoreAPI.Data;
using BookStoreAPI.DTOs;
using BookStoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Services;

public class BookService
{
    private readonly ApplicationDbContext _context;

    public BookService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Book>> GetAllAsync(string? search = null)
    {
        var query = _context.Books.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(b => b.Name.Contains(search));
        }

        return await query.ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        return await _context.Books.FindAsync(id);
    }

    public async Task<Book> CreateAsync(BookDto dto)
    {
        var book = new Book
        {
            Name = dto.Name,
            Category = dto.Category,
            Price = dto.Price,
            Description = dto.Description,
            CoverUrl = dto.CoverUrl
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return book;
    }

    public async Task<Book?> UpdateAsync(int id, BookDto dto)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return null;

        book.Name = dto.Name;
        book.Category = dto.Category;
        book.Price = dto.Price;
        book.Description = dto.Description;
        book.CoverUrl = dto.CoverUrl;

        await _context.SaveChangesAsync();
        return book;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return false;

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return true;
    }
}
