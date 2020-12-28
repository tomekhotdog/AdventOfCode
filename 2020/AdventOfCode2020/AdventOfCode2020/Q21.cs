using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020
{
    public static class Q21
    {
        public static void Solve()
        {

            var inputs = Tools.GetInput(21, false);
            var foods = inputs.Select(i => new Food(i)).ToList();
            
            
            var unknownAllergens = foods.Select(f => f.Allergens).Aggregate(
                (distinctAllergens, newSet) => distinctAllergens.Union(newSet).ToHashSet());
            var allergenMapping = new Dictionary<string, string>();
            while (true)
            {
                var derivedAllergen = DeriveAllergenMapping(unknownAllergens, foods);
                if (!derivedAllergen.HasValue) break;
                allergenMapping[derivedAllergen.Value.allergen] = derivedAllergen.Value.ingredient;
                unknownAllergens.Remove(derivedAllergen.Value.allergen);
                ReduceFoodList(foods, derivedAllergen.Value.allergen, derivedAllergen.Value.ingredient);
            } 

            var ingredientsWithNoAllergens = foods
                .Where(f => f.Allergens.Count == 0)
                .Select(f => f.Ingredients)
                .Aggregate((ingreds, newSet) => ingreds.Union(newSet).ToHashSet());

            var safeIngredientsString = string.Join(", ", ingredientsWithNoAllergens);
            Console.WriteLine($"Ingredients with no allergens: {safeIngredientsString}");
            
            // Part 1.
            var appearances = ingredientsWithNoAllergens.Sum(
                ingredient => foods.Count(food => 
                    food.Ingredients.Contains(ingredient) || food.KnownIngredients.Contains(ingredient)));
            Console.WriteLine($"Safe ingredient appearances: {appearances}");
            
            // Part 2.
            var sortedAllergenIngredientsPairs = allergenMapping.ToList();
            sortedAllergenIngredientsPairs.Sort((pair1, pair2) => 
                string.Compare(pair1.Key, pair2.Key, StringComparison.Ordinal));
            var sortedIngredients = sortedAllergenIngredientsPairs.Select(i => i.Value);
            var ingredientsString = string.Join(",", sortedIngredients);
            Console.WriteLine($"Canonical dangerous ingredient list: {ingredientsString}");
        }

        private static void ReduceFoodList(List<Food> foods, string allergen, string allergenIngredientSource)
        {
            foreach (var food in foods)
            {
                if (food.Ingredients.Contains(allergenIngredientSource))
                {
                    food.Ingredients.Remove(allergenIngredientSource);
                    food.Allergens.Remove(allergen);
                    food.KnownIngredients.Add(allergenIngredientSource);
                }
            }
        }

        private static (string allergen, string ingredient)? DeriveAllergenMapping(HashSet<string> allergens, List<Food> foods)
        {
            foreach (var allergen in allergens)
            {
                var foodsWithAllergen = foods.Where(f => f.Allergens.Contains(allergen)).ToList();
                var ingredients = foodsWithAllergen.Count == 1 ? foodsWithAllergen.First().Ingredients : 
                    foodsWithAllergen.Skip(1).Aggregate(
                        foodsWithAllergen.First().Ingredients, 
                    (set, food) => set.Intersect(food.Ingredients).ToHashSet());
                if (ingredients.Count == 1) return (allergen, ingredients.First());
            }
            return null;
        }

        public struct Food
        {
            private readonly string _raw;
            public HashSet<string> Ingredients;
            public HashSet<string> Allergens;
            public HashSet<string> KnownIngredients;

            public Food(string description)
            {
                _raw = description;
                KnownIngredients = new HashSet<string>();
                if (!description.Contains(" (")) throw new Exception($"Unexpected input: {description}");
                var ingredsAndAllergens = description.Split(" (contains ");
                Ingredients = ingredsAndAllergens[0].Split(' ').ToHashSet();
                Allergens = ingredsAndAllergens[1].Split(')')[0].Split(' ').Select(t => t.Trim(',')).ToHashSet();
            }
            
            public override string ToString()
            {
                return $"Ingredients: {string.Join(" ", Ingredients)}. Allergens: {string.Join(" ", Allergens)}.";
            }

            public override bool Equals(object? obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return _raw.GetHashCode();
            }
        }
        
    }
}