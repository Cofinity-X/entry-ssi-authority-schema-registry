/********************************************************************************
 * Copyright (c) 2024 Contributors to the Eclipse Foundation
 *
 * See the NOTICE file(s) distributed with this work for additional
 * information regarding copyright ownership.
 *
 * This program and the accompanying materials are made available under the
 * terms of the Apache License, Version 2.0 which is available at
 * https://www.apache.org/licenses/LICENSE-2.0.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 * SPDX-License-Identifier: Apache-2.0
 ********************************************************************************/

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Seeding;
using Org.Eclipse.TractusX.SsiAuthoritySchemaRegistry.Entities;
using Org.Eclipse.TractusX.SsiAuthoritySchemaRegistry.Migrations.Seeder;
using Testcontainers.PostgreSql;
using Xunit;

namespace Org.Eclipse.TractusX.SsiAuthoritySchemaRegistry.DbAccess.Tests.Setup;

[CollectionDefinition("TestDbFixture")]
public class DbTestCollection : ICollectionFixture<TestDbFixture>;

public class TestDbFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithDatabase("test_db")
        .WithImage("postgres")
        .WithCleanUp(true)
        .WithName(Guid.NewGuid().ToString())
        .Build();

    /// <summary>
    /// Foreach test a new portalDbContext will be created and filled with the custom seeding data. 
    /// </summary>
    /// <remarks>
    /// In this method the migrations don't need to get executed since they are already on the testcontainer.
    /// Because of that the EnsureCreatedAsync is enough.
    /// </remarks>
    /// <returns>Returns the created PortalDbContext</returns>
    public async Task<RegistryContext> GetDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<RegistryContext>();

        optionsBuilder.UseNpgsql(
            _container.GetConnectionString(),
            x => x.MigrationsAssembly(typeof(BatchInsertSeeder).Assembly.GetName().Name)
                .MigrationsHistoryTable("__efmigrations_history_registry")
        );
        var context = new RegistryContext(optionsBuilder.Options);
        await context.Database.EnsureCreatedAsync();

        await context.SaveChangesAsync();
        return context;
    }

    /// <summary>
    /// This method is used to initially setup the database and run all migrations
    /// </summary>
    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var optionsBuilder = new DbContextOptionsBuilder<RegistryContext>();

        optionsBuilder.UseNpgsql(
            _container.GetConnectionString(),
            x => x.MigrationsAssembly(typeof(BatchInsertSeeder).Assembly.GetName().Name)
                .MigrationsHistoryTable("__efmigrations_history_registry")
        );
        var context = new RegistryContext(optionsBuilder.Options);
        await context.Database.MigrateAsync();

        var seederOptions = Options.Create(new SeederSettings
        {
            TestDataEnvironments = ["test"],
            DataPaths = ["Seeder/Data"]
        });
        var insertSeeder = new BatchInsertSeeder(context,
            LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<BatchInsertSeeder>(),
            seederOptions);
        await insertSeeder.ExecuteAsync(CancellationToken.None);
    }

    /// <inheritdoc />
    public async Task DisposeAsync() => await _container.DisposeAsync();
}
