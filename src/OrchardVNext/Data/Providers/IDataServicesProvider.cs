using Microsoft.Data.Entity;

namespace OrchardVNext.Data.Providers {
    public interface IDataServicesProvider : ITransientDependency {
        string ProviderName { get; }
        void ConfigureContextOptions(EntityOptionsBuilder optionsBuilders);
    }

    public class SqlServerDataServicesProvider : IDataServicesProvider {
        public string ProviderName {
            get { return "SqlServer"; }
        }

        public void ConfigureContextOptions(EntityOptionsBuilder optionsBuilders) {
            optionsBuilders.UseSqlServer(@"");
        }
    }

    public class InMemoryDataServicesProvider : IDataServicesProvider {
        public string ProviderName {
            get { return "InMemory"; }
        }

        public void ConfigureContextOptions(EntityOptionsBuilder optionsBuilders) {
            optionsBuilders.UseInMemoryStore(persist: true);
        }
    }
}