using System.Collections.Generic;
#if (RSHARP8)
using JetBrains.ReSharper.Psi.Files;
#endif
// ReSharper disable RedundantUsingDirective
using System.Linq;
// ReSharper restore RedundantUsingDirective
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Html;
using JetBrains.ReSharper.Psi.Html.Tree;
using JetBrains.ReSharper.Psi.JavaScript.LanguageImpl;
using JetBrains.ReSharper.Psi.JavaScript.Tree;
using JetBrains.ReSharper.Psi.Xml;
using JetBrains.ReSharper.Psi.Xml.Tree;
using YouCantSpell.ReSharper.CSharp;
using YouCantSpell.ReSharper.Html;
using YouCantSpell.ReSharper.JavaScript;
using YouCantSpell.ReSharper.Xml;

namespace YouCantSpell.ReSharper
{

	[DaemonStage(StagesBefore = new[] { typeof(LanguageSpecificDaemonStage) })]
	public class SpellCheckDaemonStage : IDaemonStage
	{

#if RSHARP6
		private class ProcessCollection : List<IDaemonStageProcess>, IDaemonStageProcess
		{

			private readonly IDaemonProcess _process;
			
			public ProcessCollection(IDaemonProcess process, IEnumerable<IDaemonStageProcess> items)
				: base(items) {
				_process = process;
			}

			public IDaemonProcess DaemonProcess {
				get { return _process; }
			}

			public void Execute(System.Action<DaemonStageResult> committer) {
				foreach(var stage in this) {
					stage.Execute(committer);
				}
			}
		}
#endif


		private ICSharpFile GetCSharpFile(IPsiSourceFile sourceFile) {
#if RSHARP6
			return sourceFile.GetPsiFile(CSharpLanguage.Instance) as ICSharpFile;
#else
			return sourceFile.GetPsiFiles<CSharpLanguage>().OfType<ICSharpFile>().SingleOrDefault();
#endif
		}

		private IJavaScriptFile GetJavaScriptFile(IPsiSourceFile sourceFile) {
#if RSHARP6
			return sourceFile.GetPsiFile(JavaScriptLanguage.Instance) as IJavaScriptFile;
#else
			return sourceFile.GetPsiFiles<JavaScriptLanguage>().OfType<IJavaScriptFile>().SingleOrDefault();
#endif
		}

		private IXmlFile GetXmlFile(IPsiSourceFile sourceFile) {
#if RSHARP6
			return sourceFile.GetPsiFile(XmlLanguage.Instance) as IXmlFile;
#else
			return sourceFile.GetPsiFiles<XmlLanguage>().OfType<IXmlFile>().SingleOrDefault();
#endif
		}

		private static IHtmlFile GetHtmlFile(IPsiSourceFile sourceFile) {
#if RSHARP6
			return sourceFile.GetPsiFile(HtmlLanguage.Instance) as IHtmlFile;
#else
			return sourceFile.GetPsiFiles<HtmlLanguage>().OfType<IHtmlFile>().SingleOrDefault();
#endif
		}

		private List<IDaemonStageProcess> CreateProcessesCore(IDaemonProcess process, IContextBoundSettingsStore settings, DaemonProcessKind processKind) {
			var sourceFile = process.SourceFile;
			var stageProcesses = new List<IDaemonStageProcess>();

			var csFile = GetCSharpFile(sourceFile);
			if(null != csFile)
				stageProcesses.Add(new CSharpSpellCheckDaemonStageProcess(process, settings, csFile));

			var jsFile = GetJavaScriptFile(sourceFile);
			if(null != jsFile)
				stageProcesses.Add(new JavaScriptSpellCheckDaemonStageProcess(process, settings, jsFile));

			var xmlFile = GetXmlFile(sourceFile);
			if(null != xmlFile)
				stageProcesses.Add(new XmlSpellCheckDaemonStageProcess(process, xmlFile));

			var htmlFile = GetHtmlFile(sourceFile);
			if(null != htmlFile)
				stageProcesses.Add(new HtmlSpellCheckDaemonStageProcess(process, htmlFile));
			
			return stageProcesses;
		}

#if RSHARP6
		IDaemonStageProcess IDaemonStage.CreateProcess(IDaemonProcess process, IContextBoundSettingsStore settings, DaemonProcessKind processKind) {
			var items = CreateProcessesCore(process, settings, processKind);
			if(null == items || items.Count == 0)
				return null;
			if (items.Count == 1)
				return items[0];
			return new ProcessCollection(process, items);
		}
#else
		IEnumerable<IDaemonStageProcess> IDaemonStage.CreateProcess(IDaemonProcess process, IContextBoundSettingsStore settings, DaemonProcessKind processKind) {
			return CreateProcessesCore(process, settings, processKind);
		}
#endif

		public ErrorStripeRequest NeedsErrorStripe(IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore) {
			return ErrorStripeRequest.STRIPE_AND_ERRORS;
		}
	}

}
