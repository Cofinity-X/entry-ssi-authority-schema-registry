# Release Process

The release process for a new version can roughly be divided into the following steps:

- [Release Process](#release-process)
  - [Preparations on the release branch](#preparations-on-the-release-branch)
    - [1. Aggregate migrations](#1-aggregate-migrations)
    - [2. Version bump](#2-version-bump)
    - [3. Update README (on chart level)](#3-update-readme-on-chart-level)
    - [4. Update .tractusx](#4-update-tractusx)
  - [Update CHANGELOG.md](#update-changelogmd)
  - [Merge release branch](#merge-release-branch)
  - [RC: provide successive rc branch and change base of open PRs](#rc-provide-successive-rc-branch-and-change-base-of-open-prs)
  - [NOTICE](#notice)

The process builds on the [Development Flow](../dev-process/Dev-flow_git-diagram.md) which, usually, takes place within forks and leads to merged pull requests in the repositories of the eclipse-tractusx organization.

For assigning and incrementing **version** numbers [Semantic Versioning](https://semver.org) is followed.

## Preparations on the release branch

Checking out from the main branch a release branch (release/{to be released version} e.g. release/v1.2.0, or respectively release/v1.2.0-rc.1 for a release candidate).
On the release branch the following steps are executed:

### 1. Aggregate migrations

Migrations should be **aggregated in the case of releasing a new version**, in order to not release the entire history of migrations which accumulate during the development process.

Once a version has been released, migrations **mustn't be aggregated** in order to ensure upgradeability this also applies to **release candidates > rc.1 and hotfixes**.
Be aware that migrations coming release branches for release candidates or from hotfix branches, will **need to be incorporated into main**.

### 2. Version bump

The version needs to be updated in the `src` directory within the 'Directory.Build.props' file.

Also, bump the chart and app version in the [Chart.yaml](../../../charts/ssi-asr/Chart.yaml) and the version of the images in the [values.yaml](../../../charts/ssi-asr/values.yaml).

_Consortia relevant:  Update the version of the targetRevision tag in the [argocd-app-templates](../../../consortia/argocd-app-templates/), used for consortia-environments._

Example for commit message:

_build: bump version for vx.x.x_

### 3. Update README (on chart level)

Use [helm-docs](https://github.com/norwoodj/helm-docs) (gotemplate driven) for updating the README file.

```bash
helm-docs --chart-search-root [charts-dir] --sort-values-order file
```

Example for commit message:

_build: update readme for vx.x.x_

### 4. Update .tractusx

To get the latest version of the api specs build the solution once. After that adjust the links of the open api specifications in the .tractusx file.

All stable releases should be listed. In case of a pre-release version (rc, alpha, etc.), only the latest one should be listed.

```md
openApiSpecs:
- "https://raw.githubusercontent.com/eclipse-tractusx/ssi-authority-schema-registry/blob/v1.0.0/docs/api/asr-service.yaml"
- "https://raw.githubusercontent.com/eclipse-tractusx/ssi-authority-schema-registry/blob/v1.1.0/docs/api/asr-service.yaml"
```

Example for commit message:

_build: update open api spec for vx.x.x_

## Update CHANGELOG.md

The changelog file tracks all notable changes since the last released version.
Once a new version is ready to be released, the changelog can get updated via an automatically created pull request using the [release-please workflow](../../../.github/workflows/release-please.yml) which can be triggered manually or by pushing a _changelog/v*.*.*_ branch.

Please see:

- [How release please works](https://github.com/google-github-actions/release-please-action/tree/v4.0.2?tab=readme-ov-file#how-release-please-works)
- [How do I change the version number?](https://github.com/googleapis/release-please/tree/v16.7.0?tab=readme-ov-file#how-do-i-change-the-version-number)
- [How can I fix release notes?](https://github.com/googleapis/release-please/tree/v16.7.0?tab=readme-ov-file#how-can-i-fix-release-notes)

## Merge release branch

The release branch must be merged into main.
This merge needs to happen via PRs.

Example for PR titles:

_build(v1.2.0): merge release into main_

> Be aware that the merge into main triggers the workflow with the [helm-chart releaser action](../../../.github/workflows/chart-release.yaml).
>
> The workflow creates a 'ssi-authority-schema-registry-x.x.x' tag and release. The release contains the new chart.
>
> This workflow also pushes the version tag that triggers the [release workflow](../../../.github/workflows/release.yml) which creates the versioned docker image/s.


_Consortia relevant: The 'ssi-authority-schema-registry-x.x.x' tag is used to install (with the convenience of the argocd-app-templates) or upgrade the version via AgroCD on the consortia K8s clusters._

## RC: provide successive rc branch and change base of open PRs

During a release candidate phase, checkout the successive 'rc' branch and push it to the server, so that it can be used for further bugfixes.

Example:

```bash
git checkout tags/v0.1.0-rc.2 -b release/v0.1.0-rc.3
```

Also make sure to change the base of all open pull requests still pointing to the previous 'rc' branch to the newly pushed 'rc' branch.

## NOTICE

This work is licensed under the [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0).

- SPDX-License-Identifier: Apache-2.0
- SPDX-FileCopyrightText: 2024 Contributors to the Eclipse Foundation
- Source URL: <https://github.com/eclipse-tractusx/ssi-authority-schema-registry>
