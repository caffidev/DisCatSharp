# Contributing to DisCatSharp
We're really happy to accept contributions. However we also ask that you follow several rules when doing so.

# Proper base
When opening a PR, please make sure your branch targets the latest release branch, in this case it would be `main`. Also make sure your branch is even with the target branch, to avoid unnecessary surprises.

# Versioning
We follow [SemVer](https://semver.org/) versioning when it comes to pushing stable releases. Ideally, this means you should only be creating PRs for `patch` and `minor` changes. If you wish to introduce a `major` (breaking) change, please discuss it beforehand so we can determine how to integrate it into our next major version. If this involves removing a public facing property/method, mark it with the `Obsolete` attribute instead on the latest release branch.

# Proper titles
When opening issues, make sure the title reflects the purpose of the issue or the pull request. Prefer past tense, and
be brief. Further description belongs inside the issue or PR.

# Descriptive changes
We require the commits describe the change made. It can be a short description. If you fixed or resolved an open issue,
please reference it by using the # notation.

Examples of good commit messages:

* `Fixed a potential memory leak with cache entities. Fixes #142.`
* `Implemented new command extension. Resolves #169.`
* `Changed message cache behaviour. It's now global instead of per-channel.`
* `Fixed a potential NRE.`
* ```
  Changed message cache behaviour:

  - Messages are now stored globally.
  - Cache now deletes messages when they are deleted from discord.
  - Cache itself is now a ring buffer.
  ```

# Code style
We use [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)
throughout the repository, with several exceptions:

* Preference of `this`. While this one is not required, it's ill-advised to remove the existing instances thereof.
* When working with async code, and your method consists of a single `await` statement not in any `if`, `while`, etc.
  blocks, pass the task through instead of awaiting it. For example:

  ```cs
  public Task DoSomethingAsync()
    => this.DoAnotherThingAsync();

  public Task DoAnotherThingAsync()
  {
      Console.WriteLine("42");
      return this.DoYetAnotherThingAsync(42);
  }

  public async Task DoYetAnotherThingAsync(int num)
  {
      if (num == 42)
          await SuperAwesomeMethodAsync();
  }
  ```

In addition to these, we also have several preferences:

* Use initializer syntax when possible:

  ```cs
  var a = new Class
  {
      StringNumber = "fourty-two",
      Number = 42
  };

  var b = new Dictionary<string, int>()
  {
      ["fourty-two"] = 42,
      ["sixty-nine"] = 69
  };

  var c = new List<int>() { 42, 69 };

  var d = new[] { 42, 69 };
  ```
* Inline `out` declarations when possible: `SomeOutMethod(42, out var stringified);`
* Members in classes should be ordered as follows (with few exceptions):
   * Public `const` fields.
   * Non-public `const` fields.
   * Public static properties.
   * Public static fields.
   * Non-public static properties.
   * Non-public static fields.
   * Public properties.
   * Public fields.
   * Private properties.
   * Private fields.
   * Static constructor.
   * Public constructors.
   * Non-public constructors.
   * Public methods (with the exception of methods overriden from `System.Object`).
   * Non-public methods.
   * Methods overriden from `System.Object`.
   * Public static methods.
   * Non-public static methods.
   * Operator overloads.
   * Public events.
   * Non-public events.

# Code changes
One of our requirements is that all code change commits must build successfully. This is verified by our CI. When you
open a pull request, AppVeyor will start a build. You can view its summary by visiting it from the checks section on
the PR overview page.

PRs that do not build will not be accepted.

Furthermore we require that methods you implement on Discord entities have a reflection in the Discord API.

In the event your code change is a style change, XML doc change, or otherwise does not change how the code works, tag
the commit with `[ci skip]`.

# Non-code changes
In the event you change something outside of code (i.e. a meta-change or documentation), you must tag your commit with
`[ci skip]`.
