using RondoFramework.ProjectManager;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using TTS_CardTool.ViewModels;

namespace TTS_CardTool.ProjectData {
	public class DeckSerializer_V1 : IProjectModuleSerializer {
		public int Version => 1;

		public IProjectModule Deserialize(JsonNode document) {
			DeckConfig config = new DeckConfig() {
				DisplayName = document["name"]?.GetValue<string>(),
				Width = document["width"]?.GetValue<int>() ?? 0,
				Height = document["height"]?.GetValue<int>() ?? 0,
				CustomBackground = document["background"]?.GetValue<string>(),
				Font = document["font"]?.GetValue<string>() ?? "Arial",
				OutlineSize = document["outline_size"]?.GetValue<int>() ?? 5,
			};

			DeckViewModel vm = new DeckViewModel(config);

			foreach (JsonObject cardObj in document["cards"].AsArray()) {
				DeckCardViewModel card = new DeckCardViewModel() {
					Count = cardObj["count"]?.GetValue<int>() ?? 1,
				};
				foreach(KeyValuePair<string, JsonNode> cardValue in cardObj["values"].AsObject()) {
					card.CardValues[cardValue.Key] = cardValue.Value.GetValue<string>();
                }
				vm.AddNewCard(card);
			}
			return vm;
		}

		public JsonNode Serialize(IProjectModule module) {
			DeckViewModel vm = module as DeckViewModel;
			return new JsonObject() {
				["name"] = vm.DeckConfig.DisplayName,
				["width"] = vm.DeckConfig.Width,
				["height"] = vm.DeckConfig.Height,
				["background"] = vm.DeckConfig.CustomBackground,
				["font"] = vm.DeckConfig.Font,
				["outline_size"] = vm.DeckConfig.OutlineSize,
				["cards"] = CreateCardList(vm)
			};
		}

		public JsonNode UpgradeFromPreviousVersion(JsonNode document) {
			return document; // Version 1, cannot upgrade
		}

		private JsonArray CreateCardList(DeckViewModel module) {
			JsonArray parentObj = new JsonArray();
			foreach (DeckCardViewModel card in module.CardList) {
				JsonObject values = new JsonObject();
				foreach(var pair in card.CardValues) {
					values[pair.Key] = pair.Value;
                }
				parentObj.Add(new JsonObject() {
					["values"] = values,
					["count"] = card.Count
				});
			}
			return parentObj;
		}
	}
}