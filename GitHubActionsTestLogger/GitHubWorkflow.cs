﻿using System;
using System.Collections.Generic;

namespace GitHubActionsTestLogger;

// More info: https://help.github.com/en/actions/reference/workflow-commands-for-github-actions
internal static class GitHubWorkflow
{
    public static bool IsRunningOnAgent => string.Equals(
        Environment.GetEnvironmentVariable("GITHUB_ACTIONS"),
        "true",
        StringComparison.OrdinalIgnoreCase
    );

    private static string Escape(string value) =>
        // URL-encode certain characters to escape them from being processed as command tokens
        // https://pakstech.com/blog/github-actions-workflow-commands
        value
            .Replace("%", "%25")
            .Replace("\n", "%0A")
            .Replace("\r", "%0D")
            .Replace(":", "%3A")
            .Replace("=", "%3D")
            .Replace(",", "%2C");

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

    public static string FormatError(
        string title,
        string message,
        string? filePath = null,
        int? line = null,
        int? column = null) =>
        FormatWorkflowCommand("error", message, FormatOptions(filePath, line, column, title));

    public static string FormatWarning(
        string title,
        string message,
        string? filePath = null,
        int? line = null,
        int? column = null) =>
        FormatWorkflowCommand("warning", message, FormatOptions(filePath, line, column, title));
}