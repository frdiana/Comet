using CosmosDbComet.Commands;
using Spectre.Console.Cli;

var app = new CommandApp();

app.Configure(config =>
{
	config.SetApplicationName("comet");

	config.AddCommand<CreateDbCommand>("create-db")
		.WithDescription("Create a Cosmos DB database with the requested containers.");

	config.AddCommand<CloneDbCommand>("clone-db")
		.WithDescription("Clone a Cosmos DB database, optionally excluding containers.");

	config.AddCommand<InspectCommand>("inspect")
		.WithDescription("Inspect containers in a Cosmos DB database.");

	config.AddCommand<RunCommand>("run")
		.WithDescription("Run operations from a compact comet JSON file.");
});

return await app.RunAsync(args);
