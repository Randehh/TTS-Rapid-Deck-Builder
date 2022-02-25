using RondoFramework.BaseClasses;
using RondoFramework.ProjectManager;
using RondoFramework.RecentFilesTracker;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TTS_CardTool.ProjectData;
using TTS_CardTool.Views;

namespace TTS_CardTool.ViewModels {
	public class MainWindowViewModel : ViewModelBase {
		private ProjectManager m_ProjectManager;
		private RecentFilesTracker m_RecentFilesTracker = new RecentFilesTracker("TTSDeckBuilder");

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

		private SimpleCommand m_OpenRecentProjectCommand;
		public SimpleCommand OpenRecentProjectCommand {
			get => m_OpenRecentProjectCommand;
			set => SetProperty(ref m_OpenRecentProjectCommand, value);
		}

		private SimpleCommand m_SaveProjectCommand;
		public SimpleCommand SaveProjectCommand {
			get => m_SaveProjectCommand;
			set => SetProperty(ref m_SaveProjectCommand, value);
		}

		private SimpleCommand m_AboutCommand;
		public SimpleCommand AboutCommand {
			get => m_AboutCommand;
			set => SetProperty(ref m_AboutCommand, value);
		}

		private List<RecentFile> m_RecentFiles;
		public List<RecentFile> RecentFiles {
			get => m_RecentFiles;
			set => SetProperty(ref m_RecentFiles, value);
		}

		public MainWindowViewModel() {
			NewProjectCommand = new SimpleCommand((o) => CreateNewProject());
			OpenProjectCommand = new SimpleCommand((o) => LoadProject());
			OpenRecentProjectCommand = new SimpleCommand((o) => LoadProject(o as RecentFile));
			SaveProjectCommand = new SimpleCommand((o) => SaveProject(), () => ProjectData != null);
			AboutCommand = new SimpleCommand((o) => ShowAboutDialog());

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

			m_RecentFilesTracker.OnUpdate += UpdateRecentlyOpenedFiles;
			m_RecentFilesTracker.Load();
		}

		private void CreateNewProject() {
			if (!ShowCloseWarning()) {
				return;
			}

			string projectName = TextDialog.ShowDialog("New project", "Type the name of the new project.");
			if (string.IsNullOrWhiteSpace(projectName)) {
				return;
			}

			m_ProjectManager.NewProject<TTSProjectData>(projectName);
			if(m_ProjectManager.LoadedProject == null) {
				return;
			}

			m_ProjectManager.AddProjectModule(new TTSProjectViewModel() { ProjectName = projectName });
			m_ProjectManager.Save();

			m_RecentFilesTracker.AddRecentlyOpenedFile(m_ProjectManager.LoadedProject.ProjectFilePath);
		}

		private void LoadProject() {
			if (!ShowCloseWarning()) {
				return;
			}

			IProject project = m_ProjectManager.LoadFromDialog();
			LoadProject(project);
		}

		private void LoadProject(RecentFile file) {
			if (!ShowCloseWarning()) {
				return;
			}

			string path = $"{file.FullPath}.project";
			IProject project = m_ProjectManager.Load(path);
			LoadProject(project);
		}

		private void LoadProject(IProject project) {
			if (project == null) return;

			LoadedProject = ProjectData.Modules.Where((module) => module is TTSProjectViewModel).First() as TTSProjectViewModel;

			m_RecentFilesTracker.AddRecentlyOpenedFile(project.ProjectFilePath);
		}

		private void SaveProject() {
			ProjectData.RefreshDeckModules(LoadedProject);
			m_ProjectManager.Save();
		}

		private bool ShowCloseWarning() {
			if (LoadedProject == null) return true;

			DialogResult result = MessageBox.Show("Do you want to save before closing the current project?", "Closing project...", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
			switch (result) {
				case DialogResult.Yes:
					SaveProject();
					return true;

				case DialogResult.No:
					return true;

				default:
					return false;
			}
		}

		private void UpdateRecentlyOpenedFiles() {
			RecentFiles = m_RecentFilesTracker.RecentlyOpenedFiles;
		}

		private void ShowAboutDialog() {
			AboutDialog dialog = new AboutDialog() {
				Owner = System.Windows.Application.Current.MainWindow
			};
			dialog.ShowDialog();
		}
	}
}