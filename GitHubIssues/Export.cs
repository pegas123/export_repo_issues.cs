﻿using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubIssues
{
    internal static class Export
    {
        internal static void SaveAsCsv(List<Issue> issues, List<Tuple<int, IssueComment>> comments)
        {
            string path = Properties.Settings.Default.ExportFilePathAndName;
            MakeSureFileAvailable(path);

            var s = new StringBuilder("number, state, by, when, title, body");
            foreach(var issue in issues)
            {
                s.AppendLine();
                s.Append(issue.Number);
                s.Append(',');
                s.Append(issue.State.ToString());
                s.Append(',');
                s.Append(issue.User.Login);
                s.Append(',');
                s.Append(issue.CreatedAt.ToString("M/d/y"));
                s.Append(',');
                s.Append(issue.Title.EscapeText());
                s.Append(',');
                s.Append(issue.Body.EscapeText());
                if (issue.Comments > 0)
                {
                    foreach(var comment in comments.Where(x => x.Item1 == issue.Number).OrderBy(x => x.Item2.CreatedAt))
                    {
                        s.AppendLine();
                        s.Append("\"\",");
                        s.Append("\"\",");
                        s.Append(comment.Item2.User.Login);
                        s.Append(',');
                        s.Append(comment.Item2.CreatedAt.ToString("M/d/y"));
                        s.Append(',');
                        s.Append("comment");
                        s.Append(',');
                        s.Append(comment.Item2.Body.EscapeText());
                    }
                }
            }
            CreateFile(path, s.ToString());
        }

        private static void MakeSureFileAvailable(string path)
        {
            var fileName = Path.GetFileName(path);
            var pathname = Path.GetFullPath(path).Replace(fileName, "");
            if (!Directory.Exists(pathname)) System.IO.Directory.CreateDirectory(pathname);
            // Check if file already exists. If yes, delete it. 
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }
        }

        private static string CreateFile(string fileName, string contents)
        {
            try
            {
                // Create a new file 
                using (StreamWriter sw = File.CreateText(fileName))
                {
                    sw.WriteLine(contents);
                }
            }
            catch (Exception Ex)
            {
                return Ex.ToString(); // >>>>>
            }
            return "OK";
        }

        private static string EscapeText(this string s)
        {
            if (s.Any(x => x == ',' || x == '\"' || x == '\r' || x == '\n'))
            {
                return $"\"{s.Replace("\"", "\"\"")}\"";
            }
            else
            {
                return s;
            }
        }
    }
}
