﻿using System;
using System.Collections.Generic;
using System.IO;

namespace GitHubActionsTestLogger;

// Commands: https://help.github.com/en/actions/reference/workflow-commands-for-github-actions
internal partial class GitHubWorkflow
{
    private readonly TextWriter _output;

    public GitHubWorkflow(TextWriter output) => _output = output;

    private static string Escape(string value) => value
        // URL-encode certain characters to escape them from being processed as command tokens
        // https://pakstech.com/blog/github-actions-workflow-commands
        .Replace("%", "%25")
        .Replace("\n", "%0A")
        .Replace("\r", "%0D");

    private static string FormatWorkflowCommand(
        string label,
        string message,
        string options) =>
        $"::{label} {options}::{Escape(message)}";

    private static string FormatOptions(
        string? filePath = null,
        int? line = null,
        int? column = null,
        string? title = null)
    {
        var options = new List<string>(3);

        if (!string.IsNullOrWhiteSpace(filePath))
            options.Add($"file={Escape(filePath)}");

        if (line is not null)
            options.Add($"line={Escape(line.ToString())}");

        if (column is not null)
            options.Add($"col={Escape(column.ToString())}");

        if (!string.IsNullOrWhiteSpace(title))
            options.Add($"title={Escape(title)}");

        return string.Join(",", options);
    }

    public void ReportError(
        string title,
        string message,
        string? filePath = null,
        int? line = null,
        int? column = null) =>
        _output.WriteLine(FormatWorkflowCommand("error", message, FormatOptions(filePath, line, column, title)));

    public void ReportWarning(
        string title,
        string message,
        string? filePath = null,
        int? line = null,
        int? column = null) =>
        _output.WriteLine(FormatWorkflowCommand("warning", message, FormatOptions(filePath, line, column, title)));

    public void ReportSummary(string content) =>
        // There can be multiple test runs in a single step, so make sure to preserve
        // previous summaries as well.
        Environment.SetEnvironmentVariable(
            "GITHUB_STEP_SUMMARY",
            Environment.GetEnvironmentVariable("GITHUB_ACTIONS_SUMMARY") +
            Environment.NewLine +
            content,
            EnvironmentVariableTarget.Machine
        );
}

internal partial class GitHubWorkflow
{
    public static bool IsRunningOnAgent => string.Equals(
        Environment.GetEnvironmentVariable("GITHUB_ACTIONS"),
        "true",
        StringComparison.OrdinalIgnoreCase
    );
}