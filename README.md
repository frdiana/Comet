---
title: comet
description: Small C# CLI for creating and cloning Azure Cosmos DB databases
---

## comet

Fast, flashy, and it burns up on big loads. Not enterprise-ready.

comet is a small `Spectre.Console` CLI for moving small Cosmos DB
databases between accounts. It can create a database with a declared set of
containers, clone an existing database, and skip containers you do not want to
copy.

It is intentionally not designed for millions of documents, continuous sync, or
zero-downtime migrations.

## Build

```bash
dotnet build
```

Run the CLI:

```bash
comet --help
```

## Authentication

You can pass either a Cosmos DB connection string or an HTTPS account endpoint.

When you pass an endpoint, the CLI uses Azure Identity. The default auth mode is
`az-cli`, which uses the current Azure CLI login.

```bash
az login
```

The signed-in identity needs Cosmos DB data-plane permissions, such as Cosmos DB
Built-in Data Contributor, on the account or a suitable scope.

Supported auth modes:

* `az-cli`
* `default`

## Create a Database

```bash
comet create-db \
	--target "https://target.documents.azure.com:443/" \
	--auth az-cli \
	--database sales \
	--containers '["orders/tenantId","customers/customerId"]'
```

Container specs use this format:

```text
containerName/partitionKey
```

Examples:

```text
orders/tenantId
events/context/userId
```

## Clone a Database

```bash
comet clone-db \
	--source "https://source.documents.azure.com:443/" \
	--target "https://target.documents.azure.com:443/" \
	--auth az-cli \
	--database sales \
	--to sales-copy \
	--exclude-containers '["logs","audit"]' \
	--max-items 50000
```

Clone schema only:

```bash
comet clone-db \
	--source "https://source.documents.azure.com:443/" \
	--target "https://target.documents.azure.com:443/" \
	--auth az-cli \
	--database sales \
	--schema-only
```

## Inspect a Database

```bash
comet inspect \
	--source "https://source.documents.azure.com:443/" \
	--auth az-cli \
	--database sales
```

## Run from JSON

Create a compact recipe file:

```json
{
	"create": [
		{
			"target": "https://target.documents.azure.com:443/",
			"auth": "az-cli",
			"database": "sales",
			"containers": ["orders/tenantId", "customers/customerId"]
		}
	],
	"clone": [
		{
			"source": "https://source.documents.azure.com:443/",
			"target": "https://target.documents.azure.com:443/",
			"auth": "az-cli",
			"database": "sales",
			"to": "sales-copy",
			"exclude": ["logs", "audit"],
			"maxItems": 50000,
			"batchSize": 100
		}
	]
}
```

Run it:

```bash
comet run --from-file comet.json
```

Preview it without touching Cosmos DB:

```bash
comet run --from-file comet.json --dry-run
```

## Limits

comet is for small and medium moves. Use Azure-native migration tools,
custom pipelines, or change-feed based processes for large migrations,
production-grade replication, or online cutovers.

