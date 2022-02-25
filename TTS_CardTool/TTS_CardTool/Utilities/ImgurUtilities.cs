using Imgur.API.Authentication;
using Imgur.API.Endpoints;
using Imgur.API.Models;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace TTS_CardTool.Utilities {
	public class ImgurUtilities {

		public static async Task<string> UploadImage(string path) {
			ApiClient apiClient = new ApiClient("341180929098ea2927492e3cf264f35adab3fd57");
			HttpClient httpClient = new HttpClient();
			IImage result = null;

			using (var fileStream = File.OpenRead(path)) {
				ImageEndpoint imageEndpoint = new ImageEndpoint(apiClient, httpClient);
				result = await imageEndpoint.UploadImageAsync(fileStream);
			}
			return result?.Link ?? "";
		}
	}
}