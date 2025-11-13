
//legolcsobb ár
var cheapestProduct =
productsCollection.Find(_ => true).SortBy(p => p.Price).Limit(1).Project(p => p.Price).SingleOrDefault();

var cheapestCategoriesID = productsCollection.Find(x => x.Price == cheapestProduct).Project(p => p.CategoryID).ToList()

var cheapestCategories = categoriesCollecton.Find(x => cheapestCategoriesID.Contains(x.CategoryID)).ToList()

foreach (var c in cheapestCategories)
{
Console.WriteLine($"{c.Name}")
}