namespace Test.ProjectSearch.Models.Api
{
    /// <summary>
    /// Модель представления репозитория.
    /// </summary>
    public class RepositoryViewModel
    {
        /// <summary>
        /// Название проекта.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Автор.
        /// </summary>
        public string Autor { get; set; }

        /// <summary>
        /// Количество звёзд.
        /// </summary>
        public int Stargazers { get; set; }

        /// <summary>
        /// Количество наблюдателей.
        /// </summary>
        public int Watchers { get; set; }

        /// <summary>
        /// Ссылка на проект.
        /// </summary>
        public string Url { get; set; }
    }
}