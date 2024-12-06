using Application.Photos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPhotoAccessor
    {
        /// <summary>
        /// This Method is to Add Photo to the server
        /// </summary>
        /// <param name="file"></param>
        /// <returns>Photo Upload Result</returns>
        Task<PhotoUploadResult> AddPhoto(IFormFile file);

        /// <summary>
        /// This Method is to delete the photo from the server
        /// </summary>
        /// <param name="publicId"></param>
        /// <returns></returns>
        Task<string> DeletePhoto(string publicId);
    }
}
