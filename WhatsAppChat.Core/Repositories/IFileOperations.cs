using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppChat.Core.Models;
using WhatsAppChat.Data.DataModel;

namespace WhatsAppChat.Core.Repositories
{
	public interface IFileOperations
	{
		public Task<List<string>?> UploadFileAsync(FIleUploadModel model);
	}
}
