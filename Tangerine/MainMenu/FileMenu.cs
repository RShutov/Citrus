﻿using System;
using Lime;
using Tangerine.Core;
using Tangerine.UI;

namespace Tangerine
{
	public class OpenProjectCommand : Command
	{
		public OpenProjectCommand()
		{
			Text = "Open Project...";
			Shortcut = KeyBindings.Generic.OpenProject;
		}

		public override void Execute()
		{
			var dlg = new FileDialog { AllowedFileTypes = new string[] { "citproj" }, Mode = FileDialogMode.Open };
			if (dlg.RunModal()) {
				if (Project.Current.Close()) {
					new Project(dlg.FileName).Open();
					var prefs = TangerineApp.Instance.Preferences;
					prefs.RecentProjects.Remove(dlg.FileName);
					prefs.RecentProjects.Insert(0, dlg.FileName);
					prefs.Save();
				}
			}
		}
	}

	public class OpenFileCommand : Command
	{
		public OpenFileCommand()
		{
			Text = "Open File...";
			Shortcut = KeyBindings.Generic.OpenFile;
		}

		public override void Execute()
		{
			var dlg = new FileDialog { AllowedFileTypes = new string[] { "scene" }, Mode = FileDialogMode.Open };
			if (dlg.RunModal()) {
				if (!dlg.FileName.StartsWith(Project.Current.AssetsDirectory)) {
					var alert = new AlertDialog("Tangerine", "Can't open document outside the project assets directory", "Ok");
					alert.Show();
				} else {
					var path = dlg.FileName.Substring(Project.Current.AssetsDirectory.Length + 1);
					Project.Current.OpenDocument(path);
				}
			}
		}

		public override void Refresh()
		{
			Enabled = Project.Current != null;
		}
	}

	public class CloseDocumentCommand : Command
	{
		public CloseDocumentCommand()
		{
			Text = "Close Document";
			Shortcut = KeyBindings.Generic.CloseDocument;
		}

		public override void Execute()
		{
			if (Document.Current != null) {
				Project.Current.CloseDocument(Document.Current);
			}
		}
	}
}