using RondoFramework.ProjectManager;
using System.Text.Json.Nodes;
using TTS_CardTool.ViewModels;

namespace TTS_CardTool.ProjectData {
	public class TTSProjectSerializer_V1 : IProjectModuleSerializer {
		public int Version => 1;

		public IProjectModule Deserialize(JsonNode document) {
			TTSProjectViewModel vm = new TTSProjectViewModel() {
				ProjectName = document["name"]?.GetValue<string>(),
			};
			return vm;
		}

		public JsonNode Serialize(IProjectModule module) {
			TTSProjectViewModel vm = module as TTSProjectViewModel;
			return new JsonObject() {
				["name"] = vm.ProjectName,
			};
		}

		public JsonNode UpgradeFromPreviousVersion(JsonNode document) {
			return document; // Version 1, cannot upgrade
		}
	}
}
