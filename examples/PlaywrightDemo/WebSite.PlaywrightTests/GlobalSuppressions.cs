﻿// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Design",
    "CA1062:Validate arguments of public methods",
    Justification = "Unnecessary for tests",
    Scope = "namespaceanddescendants",
    Target = "~N:WebSite.PlaywrightTests.Tests"
)]
[assembly: SuppressMessage(
    "Reliability",
    "CA2007:Consider calling ConfigureAwait on the awaited task",
    Justification = "Unnecessary for tests"
)]
[assembly: SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "Required for testing.",
    Scope = "type",
    Target = "~T:WebSite.PlaywrightTests.PlaywrightFixture"
)]
[assembly: SuppressMessage(
    "Naming",
    "CA1707:Identifiers should not contain underscores",
    Justification = "Test nomenclature",
    Scope = "namespaceanddescendants",
    Target = "~N:WebSite.PlaywrightTests.Tests"
)]
[assembly: SuppressMessage(
    "Style",
    "IDE0290:Use primary constructor",
    Justification = "Unwanted recommendation"
)]
