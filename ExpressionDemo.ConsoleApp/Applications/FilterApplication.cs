using System;
using System.Linq;
using ExpressionDemo.Common;
using LinqKit;

namespace ExpressionDemo.ConsoleApp.Applications
{
    internal class FilterApplication : IApplication
    {
        private readonly IGeoDataSource _dataSource;
        private readonly IFilter _filter;

        public FilterApplication(IGeoDataSource dataSource, IFilter filter)
        {
            if (dataSource == null) throw new ArgumentNullException(nameof(dataSource));
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            _dataSource = dataSource;
            _filter = filter;
        }

        public void Run()
        {
            Func<IGeoDataLocation, bool> filterFunction = _filter.GetFilterFunction();
            _dataSource.Locations()
                .Where(filterFunction)
                .ForEach(location => Console.WriteLine("{1}: {0}", location.Name, location.CountryCode) );
        }
    }
}