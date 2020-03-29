SELECT * FROM Recipe /* gets all recipes from the Recipe table */

select * from Ingredient /* gets all Ingredients from the Recipe table */


SELECT a.Name FROM Ingredient a /* getting Name from Ingredient using alias a */
INNER JOIN RecipeIngredient b ON a.Id = b.IngredientId /* inner joining Ingredient ID to RecipeIngredent  */
WHERE b.RecipeId = 1;


UPDATE Recipe
SET Name = 'Salad'
WHERE Id = 1;


INSERT INTO Recipe
VALUES ('Salad', 50, 'KickinChicken');

DELETE FROM Recipe
WHERE Id = 3;