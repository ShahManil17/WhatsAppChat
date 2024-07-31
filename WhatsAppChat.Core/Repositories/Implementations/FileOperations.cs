using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Azure.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppChat.Core.Models;
using Azure.Storage.Blobs.Models;

namespace WhatsAppChat.Core.Repositories.Implementations
{
	public class FileOperations : IFileOperations
	{
		private readonly IConfiguration _configuration;
		public FileOperations(IConfiguration config)
        {
			_configuration = config;
		}
        public async Task<List<string>?> UploadFileAsync(FIleUploadModel model)
		{
			try
			{
				List<string>? returnStrings = new List<string>();
				if(model != null)
				{
					foreach (var item in model.Files)
					{
						string fileName = Guid.NewGuid().ToString()+item.FileName;
						using (var memoryStream = new MemoryStream())
						{
							item.CopyTo(memoryStream);
							memoryStream.Position = 0;

							var sasBuilder = new BlobSasBuilder()
							{
								BlobContainerName = "sentfiles",
								BlobName = fileName,
								Resource = "b",
								StartsOn = DateTime.UtcNow.AddMinutes(-2),
								ExpiresOn = DateTime.UtcNow.AddMinutes(2880),
							};
							sasBuilder.SetPermissions(BlobSasPermissions.Read);

							var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(_configuration.GetValue<string>("AsureConnectionString:AccountName"), _configuration.GetValue<string>("AsureConnectionString:AccessKey")));

							var newBlobClient = new BlobClient(_configuration.GetValue<string>("AsureConnectionString:ConnectionString"), "sentfiles", fileName);

							var client = await newBlobClient.UploadAsync(memoryStream, new BlobHttpHeaders { ContentType = item.ContentType });

							returnStrings.Add($"{new BlobClient(_configuration.GetValue<string>("AsureConnectionString:ConnectionString"), "sentfiles", fileName).Uri}?{sasToken}");
						}
					}
				}

				return returnStrings;
			}
			catch
			{
				return null;
			}
		}
	}
}
