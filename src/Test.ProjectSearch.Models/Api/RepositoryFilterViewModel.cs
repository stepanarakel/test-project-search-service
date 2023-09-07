namespace Test.ProjectSearch.Models.Api
{
    /// <summary>
    /// Модель представления фильтра репозиториев.
    /// </summary>
    public class RepositoryFilterViewModel
    {
        /// <summary>
        /// Текстовый запрос.
        /// </summary>
        public string TextRequest { get; set; }

        /// <summary>
        /// true - получить пустой список, false - ответ с кодом 404.
        /// </summary>
        public bool AllowEmpty { get; set; }
    }
}