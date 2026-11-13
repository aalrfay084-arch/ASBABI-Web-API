using LibaryManagement.Models;
using LibaryManagement.ResgisterDTO;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibaryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowingsController : ControllerBase
    {
        private readonly IBorrowingService _borrowingService;

        public BorrowingsController(
            IBorrowingService borrowingService)
        {
            _borrowingService = borrowingService;
        }

        // GET: api/Borrowings
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetAll()
        {
            var borrowings =
                await _borrowingService.GetAllAsync();

            return Ok(borrowings);
        }

        // GET: api/Borrowings/5
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetById(int id)
        {
            var borrowing =
                await _borrowingService.GetByIdAsync(id);

            if (borrowing == null)
                return NotFound(new
                {
                    message = "Borrowing not found."
                });

            return Ok(borrowing);
        }

        // GET: api/Borrowings/my
        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyBorrowings()
        {
            var borrowings =
                await _borrowingService.GetMyBorrowingsAsync();

            return Ok(borrowings);
        }

        // GET: api/Borrowings/my/5
        [HttpGet("my/{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetMyBorrowingById(int id)
        {
            var borrowing =
                await _borrowingService.GetMyBorrowingByIdAsync(id);

            if (borrowing == null)
                return NotFound(new
                {
                    message = "Borrowing not found."
                });

            return Ok(borrowing);
        }

        // POST: api/Borrowings
        // User: Create borrowing request
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Create(
            [FromBody] CreateBorrowingDto borrowingDto)
        {
            try
            {
                var borrowing = new Borrowing
                {
                    BookId = borrowingDto.BookId
                };

                var createdBorrowing =
                    await _borrowingService.AddAsync(borrowing);

                return CreatedAtAction(
                    nameof(GetMyBorrowingById),
                    new { id = createdBorrowing.BorrowingId },
                    createdBorrowing
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // PUT: api/Borrowings/5
        // Admin and SuperAdmin: Approve, reject, return or cancel
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateBorrowingDto borrowingDto)
        {
            try
            {
                var borrowing = new Borrowing
                {
                    Status = borrowingDto.Status
                };

                var result =
                    await _borrowingService.UpdateAsync(id, borrowing);

                if (!result)
                    return NotFound(new
                    {
                        message = "Borrowing not found."
                    });

                return Ok(new
                {
                    message = "Borrowing updated successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // DELETE: api/Borrowings/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result =
                    await _borrowingService.DeleteAsync(id);

                if (!result)
                    return NotFound(new
                    {
                        message = "Borrowing not found."
                    });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // PUT: api/Borrowings/5/pay-late-fee
        [HttpPut("{id:int}/pay-late-fee")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> PayLateFee(int id)
        {
            try
            {
                var result =
                    await _borrowingService.PayLateFeeAsync(id);

                if (!result)
                    return NotFound(
                        "عملية الاستعارة غير موجودة.");

                return Ok(
                    "تم تأكيد سداد الغرامة بنجاح.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}