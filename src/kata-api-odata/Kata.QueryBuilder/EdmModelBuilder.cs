using Microsoft.OData.Edm;

namespace Kata.QueryBuilder
{
    public class EdmModelBuilder : IEdmModelBuilder
    {
        public (IEdmModel, IEdmEntityType, IEdmEntitySet) BuildTableModel(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            var model = new EdmModel();
            var entityType = new EdmEntityType("Generic", tableName, null, false, true);
            model.AddElement(entityType);

            var defaultContainer = new EdmEntityContainer("Generic", "DefaultContainer");
            model.AddElement(defaultContainer);
            var entitySet = defaultContainer.AddEntitySet(tableName, entityType);

            return (model, entityType, entitySet);
        }
    }
}
