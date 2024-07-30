using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppChat.Core.Models
{
	public class ApiObjectModel
	{
		public List<string>? Urls { get; set; }
		public FIleUploadModel? Upload { get; set; }
	}
}
