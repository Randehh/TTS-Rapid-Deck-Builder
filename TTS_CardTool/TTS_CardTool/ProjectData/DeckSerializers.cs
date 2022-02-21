﻿using RondoFramework.ProjectManager;
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
			};

			DeckViewModel vm = new DeckViewModel(config);

			foreach (JsonObject cardObj in document["cards"].AsArray()) {
				vm.AddNewCard(new DeckCardViewModel() {
					Title = cardObj["name"]?.GetValue<string>(),
					Description = cardObj["description"]?.GetValue<string>(),
					Count = cardObj["count"]?.GetValue<int>() ?? 1,
				});
			}
			return vm;
		}

		public JsonNode Serialize(IProjectModule module) {
			DeckViewModel vm = module as DeckViewModel;
			return new JsonObject() {
				["name"] = vm.DeckConfig.DisplayName,
				["width"] = vm.DeckConfig.Width,
				["height"] = vm.DeckConfig.Height,
				["cards"] = CreateCardList(vm)
			};
		}

		public JsonNode UpgradeFromPreviousVersion(JsonNode document) {
			return document; // Version 1, cannot upgrade
		}

		private JsonArray CreateCardList(DeckViewModel module) {
			JsonArray parentObj = new JsonArray();
			foreach (DeckCardViewModel card in module.CardList) {
				parentObj.Add(new JsonObject() {
					["name"] = card.Title,
					["description"] = card.Description,
					["count"] = card.Count
				});
			}
			return parentObj;
		}
	}
}