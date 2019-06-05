﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using DreamRecorder.FoggyConsole;
using DreamRecorder.FoggyConsole.Controls;
using DreamRecorder.ToolBox.CommandLine;
using DreamRecorder.ToolBox.General;

using Microsoft.Extensions.Logging;

using WenceyWang.FIGlet;

namespace LoseChiCalendar
{



    /// <summary>
    /// 黄欣然用来找工作的玄学指导老黄历
    /// </summary>
    public sealed class Program : ApplicationBase<Program, ProgramExitCode, ProgramSetting, ProgramSettingCatalog>
    {
        private static List<string> ThingsTodo { get; set; } = new List<string>() { "a", "b", "c" };

        private static List<string> PlaceToGo { get; set; } = new List<string>() { "a", "b", "c" };

        public Program() => Name = "LoseChiCalendar";

        public static void Main(string[] args) { new Program().RunMain(args); }

        public override void ConfigureLogger(ILoggingBuilder builder)
        {
            builder.AddFilter(level => true).AddDebug();
        }

        public override void ShowLogo()
        {
            Console.WriteLine(
                            new AsciiArt(
                                        Name,
                                        width: CharacterWidth.Smush).ToString());
        }

        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public override void ShowCopyright()
        {
            Console.WriteLine($"LoseChiCalendar Copyright (C) 2019 - {DateTime.Now.Year} Xinran Huang and Wencey Wang, made with luv.");
            Console.WriteLine(@"This program comes with ABSOLUTELY NO WARRANTY.");
            Console.WriteLine(@"This is free software, and you are welcome to redistribute it under certain conditions; read License.txt for details.");
        }

        public override string License => GetLicense();

        public override bool LoadSetting => true;

        public override bool AutoSaveSetting => true;

        public override Frame PrepareViewRoot()
        {
            ViewRoot = new Frame();
            Canvas canvas = new Canvas();
            Page page = new Page
            {
                Content = canvas
            };
            ViewRoot.NavigateTo(page);

			StackPanel panel = new StackPanel ( ) ;

			canvas . Items . Add ( panel ) ;


			Label yearMonthLabel = new Label { Text = DateTime . Now . ToString ( "MMMM yyyy" ) ,HorizontalAlign=ContentHorizontalAlign.Stretch} ;

			panel . Items . Add ( yearMonthLabel ) ;


			FIGletLabel dateLabel = new FIGletLabel { Text = DateTime.Now.Day.ToString(), CharacterWidth = CharacterWidth.Smush };

			panel . Items . Add ( dateLabel ) ;


			Label dayNameLabel = new Label { Text = DateTime.Now.ToString("dddd"), HorizontalAlign = ContentHorizontalAlign.Stretch };
			panel.Items.Add(dayNameLabel);

			Button exitButton = new Button { Name = nameof ( exitButton ) , Text = "Exit", HorizontalAlign = ContentHorizontalAlign.Left,BoarderStyle=LineStyle.Empty } ;

            exitButton.Pressed += ExitButton_Pressed;

			panel . Items . Add ( exitButton ) ;

			var firstMonthDay = new DateTime ( DateTime . Now . Year , DateTime . Now . Month , 1 ) ;

			var days=DateTime . DaysInMonth ( DateTime . Now . Year , DateTime . Now . Month ) ;

			int y = 0 ;



            for ( int i = 0 ; i < days ; i++ )
			{
				for (int x = 0; x < 20; x++)
				{
					Button button = new Button { Name = $"button{i}", Text = $"{x}{y}" };
					canvas[button] = new Point(6 * x, y);
				}

			}


            return ViewRoot;

        }

        private void ExitButton_Pressed(object sender, EventArgs e) { Exit(ProgramExitCode.Success); }

		public string GetLicense() => typeof(Program).GetResourceFile(@".License.AGPL.txt");

    }

    public class ProgramSetting : SettingBase<ProgramSetting, ProgramSettingCatalog>
    {

    }

    public enum ProgramSettingCatalog
    {

        General = 0

    }

    public class ProgramExitCode : ProgramExitCode<ProgramExitCode>
    {

    }


}