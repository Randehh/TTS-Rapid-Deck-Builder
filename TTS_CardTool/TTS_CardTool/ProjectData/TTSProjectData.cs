using RondoFramework.ProjectManager;
using System.Collections.Generic;
using TTS_CardTool.ViewModels;

namespace TTS_CardTool.ProjectData {
	public class TTSProjectData : IProject {
		public string ProjectName { get; set; }
		public string ProjectPath { get; set; }
		public List<IProjectModule> Modules { get; set; } = new List<IProjectModule>();

		public void RefreshDeckModules(TTSProjectViewModel projectViewModel) {
			Modules.RemoveAll((module) => module is DeckViewModel);
			Modules.AddRange(projectViewModel.DeckList);
		}
	}
}
