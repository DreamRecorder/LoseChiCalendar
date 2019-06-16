using System ;
using System . Collections ;
using System . Collections . Generic ;
using System . Diagnostics . CodeAnalysis ;
using System . Linq ;

using DreamRecorder . FoggyConsole ;
using DreamRecorder . FoggyConsole . Controls ;
using DreamRecorder . ToolBox . CommandLine ;
using DreamRecorder . ToolBox . General ;

using LoseChiCalendar . Pages ;

using Microsoft . Extensions . Logging ;

using WenceyWang . FIGlet ;

namespace LoseChiCalendar
{

	/// <summary>
	///     黄欣然用来找工作的玄学指导老黄历
	/// </summary>
	public sealed class Program : ApplicationBase <Program , ProgramExitCode , ProgramSetting , ProgramSettingCatalog>
	{

		public override bool WaitForStart => false ;

		public override string License => GetLicense ( ) ;

		public override bool LoadSetting => true ;

		public override bool AutoSaveSetting => true ;

		public Program ( ) => Name = "LoseChiCalendar" ;

		public static void Main ( string [ ] args ) { new Program ( ) . RunMain ( args ) ; }

		public override void ConfigureLogger ( ILoggingBuilder builder )
		{
			builder . AddFilter ( level => true ) . AddDebug ( ) ;
			builder . AddFilter ( level => level >= LogLevel . Information ) . AddConsole ( ) ;
		}

		public override void ShowLogo ( )
		{
			Console . WriteLine ( new AsciiArt ( Name , width : CharacterWidth . Smush ) . ToString ( ) ) ;
		}

		[SuppressMessage ( "ReSharper" , "StringLiteralTypo" )]
		public override void ShowCopyright ( )
		{
			Console . WriteLine (
								$"LoseChiCalendar Copyright (C) 2019 - {DateTime . Now . Year} Xinran Huang and Wencey Wang, made with luv." ) ;
			Console . WriteLine ( @"This program comes with ABSOLUTELY NO WARRANTY." ) ;
			Console . WriteLine (
								@"This is free software, and you are welcome to redistribute it under certain conditions; read License.txt for details." ) ;
		}

		public override Frame PrepareViewRoot ( )
		{
			ViewRoot = new Frame ( ) ;
			Page page = new CalendarPage ( ) ;
			ViewRoot . NavigateTo ( page ) ;

			return ViewRoot ;
		}


		public string GetLicense ( ) => typeof ( Program ) . GetResourceFile ( @"License.AGPL.txt" ) ;

		public override void OnExit ( ProgramExitCode code ) => base . OnExit ( code ) ;

	}

	public class ProgramSetting : SettingBase <ProgramSetting , ProgramSettingCatalog>
	{

	}

	public enum ProgramSettingCatalog
	{

		General = 0

	}

	public class ProgramExitCode : ProgramExitCode <ProgramExitCode>
	{

	}

}
