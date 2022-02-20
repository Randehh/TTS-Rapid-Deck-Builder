using RondoFramework.BaseClasses;
using RondoFramework.ProjectManager;
using System.Collections.Generic;
using System.Linq;
using TTS_CardTool.ProjectData;
using TTS_CardTool.Views;

namespace TTS_CardTool.ViewModels {
	public class MainWindowViewModel : ViewModelBase {
		private ProjectManager m_ProjectManager;

		private TTSProjectData ProjectData => m_ProjectManager.LoadedProject as TTSProjectData;

		private TTSProjectViewModel m_LoadedProject;
		public TTSProjectViewModel LoadedProject {
			get => m_LoadedProject;
			set => SetProperty(ref m_LoadedProject, value);
		}

		private SimpleCommand m_NewProjectCommand;
		public SimpleCommand NewProjectCommand {
			get => m_NewProjectCommand;
			set => SetProperty(ref m_NewProjectCommand, value);
		}

		private SimpleCommand m_OpenProjectCommand;
		public SimpleCommand OpenProjectCommand {
			get => m_OpenProjectCommand;
			set => SetProperty(ref m_OpenProjectCommand, value);
		}

		private SimpleCommand m_SaveProjectCommand;
		public SimpleCommand SaveProjectCommand {
			get => m_SaveProjectCommand;
			set => SetProperty(ref m_SaveProjectCommand, value);
		}

		public MainWindowViewModel() {
			NewProjectCommand = new SimpleCommand((o) => { CreateNewProject(); });
			OpenProjectCommand = new SimpleCommand((o) => { LoadProject(); });
			SaveProjectCommand = new SimpleCommand((o) => { SaveProject(); }, () => ProjectData != null);

			m_ProjectManager = new ProjectManager();

			m_ProjectManager.RegisterModule<TTSProjectViewModel>(
				(project, module) => LoadedProject = module as TTSProjectViewModel,
				new List<IProjectModuleSerializer>() {
					new TTSProjectSerializer_V1(),
				});
			m_ProjectManager.RegisterModule<DeckViewModel>(
				(project, module) => LoadedProject.DeckList.Add(module as DeckViewModel),
				new List<IProjectModuleSerializer>() {
					new DeckSerializer_V1(),
				});
		}

		private void CreateNewProject() {
			if (LoadedProject != null) {
				// Show "are you sure" warning
				m_ProjectManager.Save();
			}

			string projectName = TextDialog.ShowDialog("New project", "Type the name of the new project.");
			if (string.IsNullOrWhiteSpace(projectName)) {
				return;
			}

			m_ProjectManager.NewProject<TTSProjectData>(projectName);
			m_ProjectManager.AddProjectModule(new TTSProjectViewModel() { ProjectName = projectName });
			m_ProjectManager.Save();
		}

		private void LoadProject() {
			m_ProjectManager.LoadFromDialog();
			LoadedProject = ProjectData.Modules.Where((module) => module is TTSProjectViewModel).First() as TTSProjectViewModel;
		}

		private void SaveProject() {
			ProjectData.RefreshDeckModules(LoadedProject);
			m_ProjectManager.Save();
		}
	}
}