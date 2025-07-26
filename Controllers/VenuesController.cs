using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CldvPOEnew.Models;
using CldvPOEnew.Services;
using CldvPOEnew.Models.ViewModel;

namespace CldvPOEnew.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IBlobService _blobService;

        public VenuesController(ApplicationDBContext context, IBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: Venues
        public async Task<IActionResult> Index()
        {
            return View(await _context.Venue.ToListAsync());
        }


        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venue
                .FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VenueViewModel model)
        {
            if (ModelState.IsValid)
            {
                var venue = new Venue
                {
                    VenueName = model.VenueName,
                    Location = model.Location,
                    Capacity = model.Capacity
                };

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                    var extension = Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("ImageFile", $"Only image files are allowed. Supported formats: {string.Join(", ", allowedExtensions)}");
                        return View(model);
                    }
                    try
                    {
                        venue.ImageUrl = await _blobService.UploadFileAsync(model.ImageFile);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("ImageFile", "Failed to upload image: " + ex.Message);
                        return View(model);
                    }
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // This lets the user see validation errors and correct them
            return View(model);
        }



            // GET: Venues/Edit/5
            public async Task<IActionResult> Edit(int? id)
            {
                // I check if the ID is valid
                if (id == null)
                {
                    return NotFound();
                }

                // I find the venue in the database
                var venue = await _context.Venue.FindAsync(id);
                if (venue == null)
                {
                    return NotFound();
                }

                // I convert the venue to a view model
                // This lets me handle file uploads separately from the data model
                var viewModel = new VenueViewModel
                {
                    VenueId = venue.VenueId,
                    VenueName = venue.VenueName,
                    Location = venue.Location,
                    Capacity = venue.Capacity,
                    ImageUrl = venue.ImageUrl
                };

                return View(viewModel);
            }

            // POST: Venues/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, VenueViewModel model)
            {
                // I verify the ID in the URL matches the ID in the model
                // This prevents tampering with the form
                if (id != model.VenueId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        // I find the existing venue in the database
                        var venue = await _context.Venue.FindAsync(id);
                        if (venue == null)
                        {
                            return NotFound();
                        }

                        // I update the basic properties
                        venue.VenueName = model.VenueName;
                        venue.Location = model.Location;
                        venue.Capacity = model.Capacity;

                        // If a new image was uploaded, I handle it here
                        if (model.ImageFile != null && model.ImageFile.Length > 0)
                        {
                            // Additional server-side validation for image type
                            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                            var extension = Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();

                            if (!allowedExtensions.Contains(extension))
                            {
                                ModelState.AddModelError("ImageFile", $"Only image files are allowed. Supported formats: {string.Join(", ", allowedExtensions)}");
                                return View(model);
        }
                            // First, I delete the old image if it exists
                            // This prevents orphaned files in blob storage
                            if (!string.IsNullOrEmpty(venue.ImageUrl))
                            {
                                await _blobService.DeleteFileAsync(venue.ImageUrl);
                    }

                    // Then I upload the new image
                    venue.ImageUrl = await _blobService.UploadFileAsync(model.ImageFile);
                        }

                        // I mark the entity as modified and save changes
                        _context.Update(venue);
                                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Venue updated successfully!";
                                    }
                                    catch (DbUpdateConcurrencyException)
                                    {
                        // This handles the case where someone else modified the record
                        // while this user was editing it
                        if (!VenueExists(model.VenueId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }

                // If validation fails, I return to the edit form
                return View(model);
            }
        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venue
                .FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venue.FindAsync(id);
            if (venue != null)
            {
                _context.Venue.Remove(venue);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venue.Any(e => e.VenueId == id);
        }
    }
}



//public async Task<IActionResult> Create(VenueViewModel model)
//{
//    // ModelState.IsValid checks if all validation rules pass
//    // This includes [Required] attributes, data types, etc.
//    if (ModelState.IsValid)
//    {
//        // I create a new Venue object from the view model
//        // This separation keeps my view logic separate from my data model
//        var venue = new Venue
//        {
//            VenueName = model.VenueName,
//            Location = model.Location,
//            Capacity = model.Capacity
//        };

//        // If the user uploaded an image, I handle it here
//        if (model.ImageFile != null && model.ImageFile.Length > 0)
//        {
//            // Additional server-side validation for image type
//            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
//            var extension = Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();

//            if (!allowedExtensions.Contains(extension))
//            {
//                ModelState.AddModelError("ImageFile", $"Only image files are allowed. Supported formats: {string.Join(", ", allowedExtensions)}");
//                return View(model);
//            }
//            try
//            {
//                // I use the blob storage service to upload the image to Azure
//                // This returns the URL where the image is stored
//                venue.ImageUrl = await _blobService.UploadFileAsync(model.ImageFile);
//            }
//            catch (Exception ex)
//            {
//                // If something goes wrong with the upload, I add an error to ModelState
//                // This will display an error message to the user
//                ModelState.AddModelError("ImageFile", "Failed to upload image: " + ex.Message);
//                return View(model);
//            }
//        }

//        // I add the venue to the context and save it to the database
//        _context.Add(venue);
//        await _context.SaveChangesAsync();

//        // I set a success message that will be displayed on the next page
//        TempData["SuccessMessage"] = "Venue created successfully!";

//        // After saving, I redirect to the Index page to prevent duplicate submissions
//        return RedirectToAction(nameof(Index));
//    }

//    // If validation fails, I return the same view with the model
//    // This lets the user see validation errors and correct them
//    return View(model);
//}

//// GET: Venues/Edit/5
//// This displays the form for editing an existing venue
//public async Task<IActionResult> Edit(int? id)
//{
//    // I check if the ID is valid
//    if (id == null)
//    {
//        return NotFound();
//    }

//    // I find the venue in the database
//    var venue = await _context.Venues.FindAsync(id);
//    if (venue == null)
//    {
//        return NotFound();
//    }

//    // I convert the venue to a view model
//    // This lets me handle file uploads separately from the data model
//    var viewModel = new VenueViewModel
//    {
//        VenueId = venue.VenueId,
//        VenueName = venue.VenueName,
//        Location = venue.Location,
//        Capacity = venue.Capacity,
//        ImageUrl = venue.ImageUrl
//    };

//    return View(viewModel);
//}

//// POST: Venues/Edit/5
//// This handles the form submission when editing a venue
//[HttpPost]
//[ValidateAntiForgeryToken]
//public async Task<IActionResult> Edit(int id, VenueViewModel model)
//{
//    // I verify the ID in the URL matches the ID in the model
//    // This prevents tampering with the form
//    if (id != model.VenueId)
//    {
//        return NotFound();
//    }

//    if (ModelState.IsValid)
//    {
//        try
//        {
//            // I find the existing venue in the database
//            var venue = await _context.Venues.FindAsync(id);
//            if (venue == null)
//            {
//                return NotFound();
//            }

//            // I update the basic properties
//            venue.VenueName = model.VenueName;
//            venue.Location = model.Location;
//            venue.Capacity = model.Capacity;

//            // If a new image was uploaded, I handle it here
//            if (model.ImageFile != null && model.ImageFile.Length > 0)
//            {
//                // Additional server-side validation for image type
//                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
//                var extension = Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();

//                if (!allowedExtensions.Contains(extension))
//                {
//                    ModelState.AddModelError("ImageFile", $"Only image files are allowed. Supported formats: {string.Join(", ", allowedExtensions)}");
//                    return View(model);
//                }
//                // First, I delete the old image if it exists
//                // This prevents orphaned files in blob storage
//                if (!string.IsNullOrEmpty(venue.ImageUrl))
//                {
//                    await _blobService.DeleteFileAsync(venue.ImageUrl);
//                }

//                // Then I upload the new image
//                venue.ImageUrl = await _blobService.UploadFileAsync(model.ImageFile);
//            }

//            // I mark the entity as modified and save changes
//            _context.Update(venue);
//            await _context.SaveChangesAsync();
//            TempData["SuccessMessage"] = "Venue updated successfully!";
//        }
//        catch (DbUpdateConcurrencyException)
//        {
//            // This handles the case where someone else modified the record
//            // while this user was editing it
//            if (!VenueExists(model.VenueId))
//            {
//                return NotFound();
//            }
//            else
//            {
//                throw;
//            }
//        }
//        return RedirectToAction(nameof(Index));
//    }

//    // If validation fails, I return to the edit form
//    return View(model);
//}
//'