// Copyright (c) Scott Doxey. All Rights Reserved. Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;

namespace CandyCoded.GitStatus {
    [InitializeOnLoad]
    public static class Git {
        static Git() {
            GitPath = GitSettings.GitPath;

            RepoPath = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}";
        }

        private static string GitPath { get; }

        private static string RepoPath { get; }

        public static Process GenerateProcess(string path, string arguments) {
            return Process.Start(new ProcessStartInfo {
                FileName = path,
                Arguments = arguments,
                WorkingDirectory = RepoPath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            });
        }

        public static Task<Process> GenerateProcessAsync(string path, string arguments) {
            return Task.Run(() => GenerateProcess(path, arguments));
        }

        public static async Task<string> Branch() {
            Process process = await GenerateProcessAsync(GitPath, "rev-parse --abbrev-ref HEAD");

            return process?.StandardOutput.ReadLine();
        }

        public static async Task<string[]> Branches() {
            Process process =
                await GenerateProcessAsync(GitPath, "for-each-ref --format='%(refname:short)' refs/heads");

            List<string> branches = new List<string>();

            while (process?.StandardOutput.ReadLine() is string line) {
                branches.Add(line.Trim('\''));
            }

            return branches.ToArray();
        }

        public static async Task<string[]> ChangedFiles() {
            Process process = await GenerateProcessAsync(GitPath, "status --short --untracked-files=no --porcelain");

            List<string> changes = new List<string>();

            while (process?.StandardOutput.ReadLine() is string line) {
                changes.Add(line.Trim().TrimStart('M', ' '));
            }

            return changes.ToArray();
        }

        public static async Task CheckoutBranch(string branch) {
            await GenerateProcessAsync(GitPath, $"checkout {branch}");
        }

        public static async Task DiscardChanges(string path) {
            Process process = await GenerateProcessAsync(GitPath, $@"checkout ""{path}""");

            if (process?.StandardError.ReadLine() is string line && line.StartsWith("error: pathspec")) {
                throw new("File not tracked by git.");
            }
        }

        public static async Task Init() {
            await GenerateProcessAsync(GitPath, "init");
        }
        
        public static async Task Fetch() {
            await GenerateProcessAsync(GitPath, "fetch");
        }

        public static async Task<string> Status() {
            Process process = await GenerateProcessAsync(GitPath, "status");

            if (process?.StandardError.ReadLine() is string line && line.StartsWith("fatal: not a git repository")) {
                throw new("Path is not a git repository.");
            }

            return process?.StandardOutput.ReadToEnd();
        }

        public static async Task<string[]> UntrackedFiles() {
            Process process = await GenerateProcessAsync(GitPath, "ls-files --others --exclude-standard");

            List<string> changes = new List<string>();

            while (process?.StandardOutput.ReadLine() is string line) {
                changes.Add(line.Trim());
            }

            return changes.ToArray();
        }
        
        public static async Task<string> LatestRefLocal() {
            Process process = await GenerateProcessAsync(GitPath, "rev-parse HEAD");

            return process?.StandardOutput.ReadToEnd();
        }
        
        public static async Task<string> LatestRefUpstream() {
            Process process = await GenerateProcessAsync(GitPath, "rev-parse @{u}");

            return process?.StandardOutput.ReadToEnd();
        }

        public static async Task<string> RefBase() {
            Process process = await GenerateProcessAsync(GitPath, "merge-base @ @{u}");

            return process?.StandardOutput.ReadToEnd();
        }
    }
}
#endif