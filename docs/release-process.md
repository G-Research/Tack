# Release Process

1. For this project, most of the versioning is handled by Nerdbank.GitVersioning. Check the current live version on NuGet.org and then apply the following accordingly:
- Before releasing a version with a **new feature**, we should **bump the minor version** in `version.json`.
- Before releasing a version with a **breaking change**, we should **bump the major version** in `version.json`.
- If the release neither includes a new feature nor a breaking change, then we do not change the version in `version.json`, because **nbgv already increments the patch version automatically for us**.
2. Head over to the last successful CI build logs on the main branch and retrieve the current exact build version used (X.X.X).
3. Then, draft a new [GitHub Release](https://github.com/G-Research/Tack/releases) with a list of changes contained in the new version from the [CHANGELOG](https://github.com/G-Research/Tack/blob/main/CHANGELOG.md).
4. When the draft release is ready, publish it from the GitHub web UI. You can either have GitHub create a new tag for you from the GitHub Release page or push the tag yourself beforehand. It should use the following format: "vX.X.X".
Be aware, clicking **Publish** will trigger GitHub to push a new tag (as specified in the new Release entry, if not created by you already) which will instruct the CI to build and push a new stable release to NuGet.org.
5. If everything went well, congrats, the new versions should be live on NuGet by now.