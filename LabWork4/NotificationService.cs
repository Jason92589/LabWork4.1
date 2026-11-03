namespace LabWork4
{
    public enum NotificationType
    {
        Info,
        Warning,
        Error,
        System
    }

    public enum SortNotificationBy
    {
        Date,
        Priority,
        Title
    }

    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Content { get; set; }
        public NotificationType Type { get; set; } // Info, Warning, Error, System
        public DateTime CreatedAt { get; set; }
        public int Priority { get; set; } // 0 (низкий) до 5 (высокий)
        public bool IsRead { get; set; }
    }

    public class NotificationFilterOptions
    {
        public bool? IsRead { get; set; }
        public NotificationType[]? Types { get; set; } // список нужных типов
        public string? SearchText { get; set; }
        public int? MinPriority { get; set; }
        public SortNotificationBy? SortBy { get; set; } // по Date, Priority, Title
        public bool Descending { get; set; }
    }

    public static class NotificationService
    {
        public static IEnumerable<Notification> FilterAndSort(
            IEnumerable<Notification> notifications,
            NotificationFilterOptions options)
        {
            if (notifications == null) return Enumerable.Empty<Notification>();
            if (options == null) return notifications;

            var query = notifications.AsQueryable();

            if (options.IsRead.HasValue)
            {
                query = query.Where(n => n.IsRead == options.IsRead.Value);
            }

            if (options.Types != null && options.Types.Length > 0)
            {
                query = query.Where(n => options.Types.Contains(n.Type));
            }

            if (!string.IsNullOrWhiteSpace(options.SearchText))
            {
                string search = options.SearchText.Trim();
                query = query.Where(n =>
                (n.Title != null && n.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (n.Content != null && n.Content.Contains(search, StringComparison.OrdinalIgnoreCase))
                ));
            }

            if (options.MinPriority.HasValue)
            {
                query = query.Where(n => n.Priority >= options.MinPriority.Value);
            }

            if (options.SortBy.HasValue)
            {
                if (options.SortBy.Value == SortNotificationBy.Date)
                {
                    query = options.Descending ? query.OrderByDescending(n => n.CreatedAt) : query.OrderBy(n => n.CreatedAt);
                }
                else if (options.SortBy.Value == SortNotificationBy.Priority)
                {
                    query = options.Descending ? query.OrderByDescending(n => n.Priority) : query.OrderBy(n => n.Priority);
                }
                else if (options.SortBy.Value == SortNotificationBy.Title)
                {
                    query = options.Descending ? query.OrderByDescending(n => n.Title) : query.OrderBy(n => n.Title);
                }
            }

            return query.ToList();
        }

    }

}
