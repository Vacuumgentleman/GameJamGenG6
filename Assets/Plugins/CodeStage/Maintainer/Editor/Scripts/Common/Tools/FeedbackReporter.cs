namespace CodeStage.EditorCommon.Tools
{
	using System;
	using System.Text;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Opens a pre-filled issue form in the product's public feedback repository.
	/// Ships no credentials: the customer submits the issue under their own GitHub account,
	/// which also gives them a public issue they can follow for progress.
	/// </summary>
	internal class FeedbackReporter
	{
		private const string BugTemplate = "bug_report.yml";
		private const string FeatureTemplate = "feature_request.yml";
		private const int MaxUrlLength = 7500;

		private readonly string repositoryUrl;
		private readonly string productVersion;

		public FeedbackReporter(string repositoryUrl, string productVersion)
		{
			if (string.IsNullOrEmpty(repositoryUrl))
				throw new ArgumentNullException("repositoryUrl");

			this.repositoryUrl = repositoryUrl.TrimEnd('/');
			this.productVersion = productVersion ?? string.Empty;
		}

		public void ReportBug()
		{
			Application.OpenURL(BuildBugUrl());
		}

		public void RequestFeature()
		{
			Application.OpenURL(BuildFeatureUrl());
		}

		/// <summary>
		/// Opens the list of existing reports so the user can check for a duplicate
		/// or follow something they filed earlier.
		/// </summary>
		public void OpenTracker()
		{
			Application.OpenURL(repositoryUrl + "/issues");
		}

		public string BuildBugUrl()
		{
			var builder = StartUrl(BugTemplate);
			Append(builder, "asset_version", productVersion);
			Append(builder, "unity_version", Application.unityVersion);
			Append(builder, "platform", GetActivePlatform());
			return Finish(builder, BugTemplate);
		}

		public string BuildFeatureUrl()
		{
			var builder = StartUrl(FeatureTemplate);
			Append(builder, "asset_version", productVersion);
			return Finish(builder, FeatureTemplate);
		}

		private StringBuilder StartUrl(string template)
		{
			var builder = new StringBuilder();
			builder.Append(repositoryUrl).Append("/issues/new?template=").Append(template);
			return builder;
		}

		/// <summary>
		/// GitHub only accepts pre-filled values through the query string, so an over-long URL
		/// would be rejected. Falling back to the bare template keeps the button working.
		/// </summary>
		private string Finish(StringBuilder builder, string template)
		{
			if (builder.Length > MaxUrlLength)
				return repositoryUrl + "/issues/new?template=" + template;

			return builder.ToString();
		}

		private static void Append(StringBuilder builder, string field, string value)
		{
			if (string.IsNullOrEmpty(value))
				return;

			builder.Append('&').Append(field).Append('=').Append(Uri.EscapeDataString(value));
		}

		/// <summary>
		/// Maps the active build target onto the options offered by the shared issue form.
		/// Matched by name rather than by enum member, since console and platform members
		/// come and go between the Unity versions these packages support.
		/// Anything unrecognised is left for the reporter to pick.
		/// </summary>
		private static string GetActivePlatform()
		{
			var target = EditorUserBuildSettings.activeBuildTarget.ToString();

			if (target.StartsWith("StandaloneWindows", StringComparison.Ordinal))
				return "Windows";
			if (target.StartsWith("StandaloneOSX", StringComparison.Ordinal))
				return "macOS";
			if (target.StartsWith("StandaloneLinux", StringComparison.Ordinal))
				return "Linux";
			if (target == "Android")
				return "Android";
			if (target == "iOS")
				return "iOS";
			if (target == "WebGL")
				return "WebGL";
			if (target.StartsWith("PS", StringComparison.Ordinal) ||
			    target.StartsWith("Xbox", StringComparison.Ordinal) ||
			    target.StartsWith("GameCore", StringComparison.Ordinal) ||
			    target == "Switch")
				return "Console";

			return string.Empty;
		}
	}
}
