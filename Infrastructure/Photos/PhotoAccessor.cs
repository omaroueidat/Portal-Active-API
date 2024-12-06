using Application.Interfaces;
using Application.Photos;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Photos
{
    public class PhotoAccessor : IPhotoAccessor
    {
        private readonly Cloudinary _cloudinary;
        public PhotoAccessor(IOptions<CloudinarySettings> config)
        {
            // Cloudinary Account for image cloud service
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            // Set the value of the cloudinary
            _cloudinary = new Cloudinary( account );

        }

        public async Task<PhotoUploadResult> AddPhoto(IFormFile file)
        {
            // Check file exist
            if (file.Length > 0)
            {
                // Dispose the file
                await using var stream = file.OpenReadStream();

                // Upload Parameters where it will apply on each image when uploading
                var uploadParams = new ImageUploadParams
                {
                    // Name of the file and the stream of bytes
                    File = new FileDescription(file.FileName, stream),

                    // Convert the image to square
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill"),
                };

                // Upload the image and get the result
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                // Check for errors
                if (uploadResult.Error is not null)
                {
                    throw new Exception(uploadResult.Error.Message);
                }

                // Return the result custom class by mathcing attributes
                return new PhotoUploadResult
                {
                    PublicId = uploadResult.PublicId,
                    Url = uploadResult.SecureUrl.ToString(),
                };
            }

            return null;
        }

        public async Task<string> DeletePhoto(string publicId)
        {
            // Prepare the Prameters for the deletion of the photo
            var deleteParams = new DeletionParams(publicId);

            // Delete the photo and revcieve a response
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result.Result == "ok" ? result.Result : null;
        }
    }
}
