using System;
using System.Collections.Generic;
using System.Linq;

namespace Exam.PackageManagerLite
{
    public class PackageManager : IPackageManager
    {
        Dictionary<string, Package> packagesById = new Dictionary<string, Package>();
        Dictionary<string, Package> packagesByName = new Dictionary<string, Package>();

        public void AddDependency(string packageId, string dependencyId)
        {
            if (!packagesById.ContainsKey(packageId) || !packagesById.ContainsKey(dependencyId))
            {
                throw new ArgumentException();
            }

            packagesById[packageId].Dependencies.Add(packagesById[dependencyId]);
        }

        public bool Contains(Package package)
            => packagesById.ContainsKey(package.Id);

        public int Count()
            => packagesById.Count;

        public IEnumerable<Package> GetDependants(Package package)
            => packagesById.Values
                .Where(p => p.Dependencies.Contains(package));

        public IEnumerable<Package> GetIndependentPackages()
            => packagesById.Values
                .Where(p => p.Dependencies.Count == 0)
                .OrderByDescending(p => p.ReleaseDate)
                .ThenBy(p => p.Version);

        public IEnumerable<Package> GetOrderedPackagesByReleaseDateThenByVersion()
        {
            var distinct = packagesById.Values
                    .GroupBy(p => new { p.Name, p.ReleaseDate })
                    .Select(p => p.First())
                    .ToList();

            return distinct
                .OrderByDescending(p => p.ReleaseDate)
                .ThenBy(p => p.Version);
        }

        public void RegisterPackage(Package package)
        {
            if (packagesByName.ContainsKey(package.Name) && packagesById[package.Name].Version == package.Version)
            {
                throw new ArgumentException();
            }

            packagesById.Add(package.Id, package);
            packagesByName.Add(package.Name, package);
        }

        public void RemovePackage(string packageId)
        {
            if (!packagesById.ContainsKey(packageId))
            {
                throw new ArgumentException();
            }

            var removed = packagesById[packageId];
            packagesById.Remove(removed.Id);
            packagesByName.Remove(removed.Name);

            foreach (var package in packagesById.Values)
            {
                if (package.Dependencies.Contains(removed))
                {
                    package.Dependencies.Remove(removed);
                }
            }
        }
    }
}
