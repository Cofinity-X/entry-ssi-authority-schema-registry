# Dev flow with deployment to dev environment

```mermaid
flowchart LR
    subgraph local
    D(Developer)
    end
    subgraph eclipse-tractusx
        direction LR
        D -- PR* to main*--> SASR(ssi-authority-schema-registry**)
        click SCI "https://github.com/eclipse-tractusx/ssi-authority-schema-registry"
    end
    subgraph Argo CD - sync to k8s cluster
    SASR -- auto-sync --> A(Argo CD dev)
    end
```

Note\* Every pull request (PR) requires at least one approving review by a committer

Note\*\* Unit tests and code analysis checks run at pull request

## NOTICE

This work is licensed under the [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0).

- SPDX-License-Identifier: Apache-2.0
- SPDX-FileCopyrightText: 2024 Contributors to the Eclipse Foundation
- Source URL: <https://github.com/eclipse-tractusx/ssi-authority-schema-registry>
