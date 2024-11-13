﻿using Microsoft.AspNetCore.Mvc;
using ServicesForShelfSwap.Data;
using ServicesForShelfSwap.Models;
using System;
using System.Threading.Tasks;
using ServicesForShelfSwap.Data; // Replace with your actual namespace
using ServicesForShelfSwap.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // Replace with your actual namespace


[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly BookExchangeContext _context;

    public BooksController(BookExchangeContext context)
    {
        _context = context;
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddBook([FromBody] Book book)
    {
        if (book == null)
        {
            return BadRequest(new { message = "Book data is required." });
        }

        book.CreatedAt = DateTime.UtcNow; // Set the created date
        book.UpdatedAt = DateTime.UtcNow; // Set the updated date

        _context.Books.Add(book);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Book added successfully.",
            bookId = book.BookId // Assuming BookId is generated by the database
        });
    }
    [Authorize]
    [HttpPut("{bookId}")]
    public async Task<IActionResult> EditBook(int bookId, [FromBody] Book updatedBook)
    {
        // Check if the provided updated book details are valid
        if (updatedBook == null)
        {
            return BadRequest(new { message = "Book data is required." });
        }

        // Find the book by its BookId
        var existingBook = await _context.Books.FindAsync(bookId);
        if (existingBook == null)
        {
            return NotFound(new { message = "Book not found." });
        }

        // Update the book details
        existingBook.Title = updatedBook.Title;
        existingBook.Author = updatedBook.Author;
        existingBook.Genre = updatedBook.Genre;
        existingBook.Condition = updatedBook.Condition;
        existingBook.AvailabilityStatus = updatedBook.AvailabilityStatus;
        existingBook.UpdatedAt = DateTime.UtcNow; // Update the timestamp
        existingBook.Location = updatedBook.Location;
        // Save the changes to the database
        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { message = "Book details updated successfully." });
        }
        catch (DbUpdateException ex)
        {
            // Log or handle the exception as needed
            return StatusCode(500, new { message = "Error updating book details.", details = ex.Message });
        }
    }
    [Authorize]
    [HttpDelete("{bookId}")]
    public async Task<IActionResult> DeleteBook(int bookId)
    {
        // Find the book by its BookId
        var book = await _context.Books.FindAsync(bookId);
        if (book == null)
        {
            return NotFound(new { message = "Book not found." });
        }

        // Remove the book from the database
        _context.Books.Remove(book);

        // Save the changes
        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { message = "Book deleted successfully." });
        }
        catch (DbUpdateException ex)
        {
            // Log or handle the exception as needed
            return StatusCode(500, new { message = "Error deleting book.", details = ex.Message });
        }
    }
    [Authorize]
    [HttpGet("search")]
    public async Task<IActionResult> SearchBooks(
    [FromQuery] string? title,
    [FromQuery] string? author,
    [FromQuery] string? genre,
    [FromQuery] string? location,
    [FromQuery] string? availabilityStatus)
    {
        // Start with a queryable collection of books
        var query = _context.Books.AsQueryable();

        // Apply filters based on query parameters
        if (!string.IsNullOrEmpty(title))
        {
            query = query.Where(b => b.Title.Contains(title));
        }

        if (!string.IsNullOrEmpty(author))
        {
            query = query.Where(b => b.Author.Contains(author));
        }

        if (!string.IsNullOrEmpty(genre))
        {
            query = query.Where(b => b.Genre.Contains(genre));
        }

        if (!string.IsNullOrEmpty(location))
        {
            query = query.Where(b => b.Location.Contains(location));
        }

        if (!string.IsNullOrEmpty(availabilityStatus))
        {
            query = query.Where(b => b.AvailabilityStatus.Equals(availabilityStatus, StringComparison.OrdinalIgnoreCase));
        }

        // Execute the query and retrieve the list of filtered books
        var books = await query
            .Select(b => new
            {
                bookId = b.BookId,
                title = b.Title,
                author = b.Author,
                genre = b.Genre,
                condition = b.Condition,
                availabilityStatus = b.AvailabilityStatus,
                userId = b.UserId
            })
            .ToListAsync();

        return Ok(new { books });
    }

    // Get books of a particular user
    [Authorize]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetBooksByUser(int userId)
    {
        // Retrieve books where the UserId matches the specified userId
        var books = await _context.Books
            .Where(b => b.UserId == userId)
            .Select(b => new
            {
                bookId = b.BookId,
                title = b.Title,
                author = b.Author,
                genre = b.Genre,
                condition = b.Condition,
                availabilityStatus = b.AvailabilityStatus,
                location = b.Location,
                createdAt = b.CreatedAt,
                updatedAt = b.UpdatedAt
            })
            .ToListAsync();

        return Ok(new { books });
    }

    // Get all books except those of a particular user
    [Authorize]
    [HttpGet("excludeUser/{userId}")]
    public async Task<IActionResult> GetBooksExcludingUser(int userId)
    {
        // Retrieve books where the UserId does not match the specified userId
        var books = await _context.Books
            .Where(b => b.UserId != userId)
            .Select(b => new
            {
                bookId = b.BookId,
                title = b.Title,
                author = b.Author,
                genre = b.Genre,
                condition = b.Condition,
                availabilityStatus = b.AvailabilityStatus,
                location = b.Location,
                createdAt = b.CreatedAt,
                updatedAt = b.UpdatedAt
            })
            .ToListAsync();

        return Ok(new { books });
    }

}
