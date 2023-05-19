﻿using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using GitHubActionsTestLogger.Tests.Fakes;
using Xunit;

namespace GitHubActionsTestLogger.Tests;

public class InitializationSpecs
{
    [Fact]
    public void I_can_use_the_logger_with_the_default_configuration()
    {
        // Arrange
        var logger = new TestLogger();
        var events = new FakeTestLoggerEvents();

        // Act
        logger.Initialize(events, Directory.GetCurrentDirectory());

        // Assert
        logger.Context.Should().NotBeNull();
        logger.Context?.Options.Should().Be(TestLoggerOptions.Default);
    }

    [Fact]
    public void I_can_use_the_logger_with_a_custom_configuration()
    {
        // Arrange
        var logger = new TestLogger();

        var events = new FakeTestLoggerEvents();
        var parameters = new Dictionary<string, string?>
        {
            ["annotations.titleFormat"] = "TitleFormat",
            ["annotations.messageFormat"] = "MessageFormat",
            ["summary.compactLayout"] = "true",
            ["summary.includePassedTests"] = "true",
            ["summary.includeSkippedTests"] = "true"
        };

        // Act
        logger.Initialize(events, parameters);

        // Assert
        logger.Context.Should().NotBeNull();
        logger.Context?.Options.AnnotationTitleFormat.Should().Be("TitleFormat");
        logger.Context?.Options.AnnotationMessageFormat.Should().Be("MessageFormat");
        logger.Context?.Options.SummaryCompactLayout.Should().BeTrue();
        logger.Context?.Options.SummaryIncludePassedTests.Should().BeTrue();
        logger.Context?.Options.SummaryIncludeSkippedTests.Should().BeTrue();
    }
}