using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;  
using System.Threading.Tasks;
using Moq;
using NotesApp.Application.DTOs;
using NotesApp.Application.Interfaces;
using NotesApp.Application.Services;
using NotesApp.Domain.Entities;
using Xunit;

namespace NotesApp.Tests.Services
{
    public class NoteServiceTests
    {
        [Fact]
        public async Task CreateNoteAsync_ShouldReturnCreatedNote_WhenDtoIsValid()
        {
            // Arrange
            var mockRepo = new Mock<INoteRepository>();

            Note capturedNote = null;
            mockRepo
                .Setup(r => r.AddAsync(It.IsAny<Note>()))
                .Callback<Note>(n =>
                {
                    // simulate repository assigning an Id
                    n.Id = 42;
                    capturedNote = n;
                })
                .Returns(Task.CompletedTask);

            mockRepo
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // NOTE: The production NoteService is expected to accept INoteRepository in its constructor.
            var service = new NoteService(mockRepo.Object);

            var userId = 7;
            var dto = new NoteCreateDto
            {
                Title = "Test Title",
                Content = "Test content"
            };

            // Act
            var beforeCall = DateTime.UtcNow;
            var result = await service.CreateNoteAsync(userId, dto);
            var afterCall = DateTime.UtcNow;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(42, result.Id); // repository-assigned id
            Assert.Equal(dto.Title, result.Title);
            Assert.Equal(dto.Content, result.Content);
            Assert.Equal(userId, result.UserId);

            // CreatedAt and UpdatedAt should be set to a recent timestamp
            Assert.True(result.CreatedAt >= beforeCall && result.CreatedAt <= afterCall);
            Assert.True(result.UpdatedAt >= beforeCall && result.UpdatedAt <= afterCall);

            // Repository interactions
            mockRepo.Verify(r => r.AddAsync(It.IsAny<Note>()), Times.Once);
            mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteNoteAsync_ShouldReturnTrueAndCallRepository_WhenNoteExists()
        {
            // Arrange
            var mockRepo = new Mock<INoteRepository>();

            var note = new Note
            {
                Id = 3,
                UserId = 7,
                Title = "Title",
                Content = "Content",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            mockRepo
                .Setup(r => r.GetNoteByIdAsync(7, 3))
                .ReturnsAsync(note);

            mockRepo
                .Setup(r => r.DeleteAsync(It.IsAny<Note>()))
                .Returns(Task.CompletedTask);

            mockRepo
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var service = new NoteService(mockRepo.Object);

            // Act
            var result = await service.DeleteNoteAsync(7, 3);

            // Assert
            Assert.True(result);
            mockRepo.Verify(r => r.GetNoteByIdAsync(7, 3), Times.Once);
            mockRepo.Verify(r => r.DeleteAsync(It.Is<Note>(n => n == note)), Times.Once);
            mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteNoteAsync_ShouldReturnFalseAndNotCallDelete_WhenNoteDoesNotExist()
        {
            // Arrange
            var mockRepo = new Mock<INoteRepository>();

            mockRepo
                .Setup(r => r.GetNoteByIdAsync(7, 3))
                .ReturnsAsync((Note?)null);

            var service = new NoteService(mockRepo.Object);

            // Act
            var result = await service.DeleteNoteAsync(7, 3);

            // Assert
            Assert.False(result);
            mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Note>()), Times.Never);
            mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task GetNotesAsync_ShouldReturnNotesForUser()
        {
            // Arrange
            var mockRepo = new Mock<INoteRepository>();

            var notes = new[]
            {
                new Note { Id = 1, UserId = 7, Title = "A", Content = "a", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Note { Id = 2, UserId = 7, Title = "B", Content = "b", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };

            mockRepo
                .Setup(r => r.GetNotesByUserAsync(7))
                .ReturnsAsync(notes);

            var service = new NoteService(mockRepo.Object);

            // Act
            var result = await service.GetNotesAsync(7);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, n => n.Id == 1);
            Assert.Contains(result, n => n.Id == 2);
            mockRepo.Verify(r => r.GetNotesByUserAsync(7), Times.Once);
        }

        [Fact]
        public async Task GetNoteByIdAsync_ShouldReturnNote_WhenExists()
        {
            // Arrange
            var mockRepo = new Mock<INoteRepository>();

            var note = new Note
            {
                Id = 3,
                UserId = 7,
                Title = "Title",
                Content = "Content",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            mockRepo
                .Setup(r => r.GetNoteByIdAsync(7, 3))
                .ReturnsAsync(note);

            var service = new NoteService(mockRepo.Object);

            // Act
            var result = await service.GetNoteByIdAsync(7, 3);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Id);
            Assert.Equal(7, result.UserId);
            mockRepo.Verify(r => r.GetNoteByIdAsync(7, 3), Times.Once);
        }

        [Fact]
        public async Task GetNoteByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            var mockRepo = new Mock<INoteRepository>();

            mockRepo
                .Setup(r => r.GetNoteByIdAsync(7, 99))
                .ReturnsAsync((Note?)null);

            var service = new NoteService(mockRepo.Object);

            // Act
            var result = await service.GetNoteByIdAsync(7, 99);

            // Assert
            Assert.Null(result);
            mockRepo.Verify(r => r.GetNoteByIdAsync(7, 99), Times.Once);
        }

        [Fact]
        public async Task UpdateNoteAsync_ShouldReturnUpdatedNote_WhenDtoValid()
        {
            // Arrange
            var mockRepo = new Mock<INoteRepository>();

            var note = new Note
            {
                Id = 5,
                UserId = 7,
                Title = "Old",
                Content = "Old content",
                CreatedAt = DateTime.UtcNow.AddMinutes(-10),
                UpdatedAt = DateTime.UtcNow.AddMinutes(-10)
            };

            mockRepo
                .Setup(r => r.GetNoteByIdAsync(7, 5))
                .ReturnsAsync(note);

            mockRepo
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var service = new NoteService(mockRepo.Object);

            var dto = new NoteUpdateDto
            {
                Title = "New Title",
                Content = "New Content"
            };

            // Act
            var beforeCall = DateTime.UtcNow;
            var result = await service.UpdateNoteAsync(7, 5, dto);
            var afterCall = DateTime.UtcNow;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Id);
            Assert.Equal("New Title", result.Title);
            Assert.Equal("New Content", result.Content);
            Assert.True(result.UpdatedAt >= beforeCall && result.UpdatedAt <= afterCall);
            mockRepo.Verify(r => r.GetNoteByIdAsync(7, 5), Times.Once);
            mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateNoteAsync_ShouldReturnNull_WhenNoteNotFound()
        {
            // Arrange
            var mockRepo = new Mock<INoteRepository>();

            mockRepo
                .Setup(r => r.GetNoteByIdAsync(7, 42))
                .ReturnsAsync((Note?)null);

            var service = new NoteService(mockRepo.Object);

            var dto = new NoteUpdateDto
            {
                Title = "X",
                Content = "Y"
            };

            // Act
            var result = await service.UpdateNoteAsync(7, 42, dto);

            // Assert
            Assert.Null(result);
            mockRepo.Verify(r => r.GetNoteByIdAsync(7, 42), Times.Once);
            mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}
