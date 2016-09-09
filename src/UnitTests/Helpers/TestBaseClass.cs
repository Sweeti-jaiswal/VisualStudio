﻿using EntryExitDecoratorInterfaces;
using GitHub.Models;
using Octokit;
using System;
using System.IO;
using Xunit.Abstractions;

/// <summary>
/// This base class will get its methods called by the most-derived
/// classes. The calls are injected by the EntryExitMethodDecorator Fody
/// addin, so don't be surprised if you don't see any calls in the code.
/// </summary>
public class TestBaseClass : IEntryExitDecorator
{
    public virtual void OnEntry()
    {
        // Ensure that every test has the InUnitTestRunner flag
        // set, so threading doesn't go nuts.
        Splat.ModeDetector.Current.SetInUnitTestRunner(true);
    }

    public virtual void OnExit()
    {
    }

    protected static UserAndScopes CreateUserAndScopes(string login, string[] scopes = null)
    {
        return new UserAndScopes(CreateOctokitUser(login), scopes);
    }

    protected static User CreateOctokitUser(string login = "login", string url = "https://url")
    {
        return new User("https://url", "bio", "blog", 1, "GitHub",
            DateTimeOffset.UtcNow, 0, "email", 100, 100, true, url,
            10, 42, "location", login, "name", 1, new Plan(),
            1, 1, 1, "https://url", new RepositoryPermissions(true, true, true),
            false, null, null);
    }

    protected static Organization CreateOctokitOrganization(string login)
    {
        return new Organization("https://url", "", "", 1, "GitHub", DateTimeOffset.UtcNow, 0, "email", 100, 100, true, "http://url", 10, 42, "somewhere", login, "Who cares", 1, new Plan(), 1, 1, 1, "https://url", "billing");
    }

    protected static Repository CreateRepository(string owner, string name, string domain = "github.com", long id = 1, Repository parent = null)
    {
        var cloneUrl = "https://" + domain + "/" + owner + "/" + name;
        string notCloneUrl = cloneUrl + "-x";
        return new Repository(notCloneUrl, notCloneUrl, cloneUrl, notCloneUrl, notCloneUrl, notCloneUrl, notCloneUrl,
            id, CreateOctokitUser(owner),
            name, "fullname", "description", notCloneUrl, "c#", false, parent != null, 0, 0, "master",
            0, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow,
            new RepositoryPermissions(), parent, null, true, false, false);
    }

    protected static PullRequest CreatePullRequest(User user, int id, ItemState state, string title,
        DateTimeOffset createdAt, DateTimeOffset updatedAt, int commentCount = 0, int reviewCommentCount = 0)
    {
        var uri = new Uri("https://url");
        return new PullRequest(uri, uri, uri, uri, uri, uri,
            id, state, title, "", createdAt, updatedAt,
            null, null, null, null, user, null, false, null,
            commentCount, reviewCommentCount, 0, 0, 0, 0,
            null, false);
    }

    protected class TempDirectory : IDisposable
    {
        ITestOutputHelper output;

        public TempDirectory(ITestOutputHelper output = null)
        {
            this.output = output;
            var f = Path.GetTempFileName();
            var name = Path.GetFileNameWithoutExtension(f);
            File.Delete(f);
            Directory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), name));
            Directory.Create();
            output?.WriteLine("Created " + Directory);
        }

        public DirectoryInfo Directory { get; }

        public void Dispose()
        {
            output?.WriteLine("Deleted " + Directory);
            Directory.Delete(true);
        }
    }
}
