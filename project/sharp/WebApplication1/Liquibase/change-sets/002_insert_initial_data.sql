--liquibase formatted sql

--changeset nika:002_insert_initial_data
INSERT INTO roles (name, description) VALUES
    ('Admin', 'Full system access'),
    ('Manager', 'Can manage trips and journals'),
    ('User', 'Can view and create own trips')
ON CONFLICT (name) DO NOTHING;

INSERT INTO permissions (name, description) VALUES
    ('trips.read', 'Read trips'),
    ('trips.create', 'Create trips'),
    ('trips.update', 'Update trips'),
    ('trips.delete', 'Delete trips'),
    ('journals.read', 'Read journals'),
    ('journals.create', 'Create journals'),
    ('journals.update', 'Update journals'),
    ('journals.delete', 'Delete journals'),
    ('locations.read', 'Read locations'),
    ('locations.create', 'Create locations'),
    ('locations.update', 'Update locations'),
    ('locations.delete', 'Delete locations')
ON CONFLICT (name) DO NOTHING;

INSERT INTO role_permissions (role_id, permission_id)
SELECT r.id, p.id
FROM roles r
CROSS JOIN permissions p
WHERE r.name = 'Admin'
ON CONFLICT DO NOTHING;

INSERT INTO role_permissions (role_id, permission_id)
SELECT r.id, p.id
FROM roles r
CROSS JOIN permissions p
WHERE r.name = 'Manager' 
  AND (p.name LIKE '%.read' OR p.name LIKE '%.create' OR p.name LIKE '%.update')
ON CONFLICT DO NOTHING;

INSERT INTO role_permissions (role_id, permission_id)
SELECT r.id, p.id
FROM roles r
CROSS JOIN permissions p
WHERE r.name = 'User' 
  AND (p.name LIKE '%.read' OR p.name LIKE '%.create')
ON CONFLICT DO NOTHING;

INSERT INTO users (username, email, password_hash, role_id) VALUES
    ('admin', 'admin@traveljournal.com', 'UV3TtgQAqW7xaKsCfcHte5blKPvZ+gPPfPxzRHAbWVg=', 
     (SELECT id FROM roles WHERE name = 'Admin'))
ON CONFLICT (username) DO NOTHING;

INSERT INTO locations (name, country, city, latitude, longitude, description)
SELECT * FROM (VALUES
    ('Эйфелева башня', 'Франция', 'Париж', 48.8584::decimal, 2.2945::decimal, 'Знаменитая железная башня'),
    ('Биг-Бен', 'Великобритания', 'Лондон', 51.4994::decimal, -0.1245::decimal, 'Историческая часовая башня'),
    ('Колизей', 'Италия', 'Рим', 41.8902::decimal, 12.4922::decimal, 'Древний амфитеатр'),
    ('Саграда Фамилия', 'Испания', 'Барселона', 41.4036::decimal, 2.1744::decimal, 'Незавершенная базилика'),
    ('Акрополь', 'Греция', 'Афины', 37.9715::decimal, 23.7267::decimal, 'Древняя цитадель')
) AS v(name, country, city, latitude, longitude, description)
WHERE NOT EXISTS (SELECT 1 FROM locations l WHERE l.name = v.name);

INSERT INTO tags (name, color) VALUES
    ('Приключения', '#e74c3c'),
    ('Отдых', '#3498db'),
    ('Культура', '#9b59b6'),
    ('Природа', '#2ecc71'),
    ('Еда', '#f39c12'),
    ('История', '#34495e')
ON CONFLICT (name) DO NOTHING;

INSERT INTO api_keys (key_hash, name, description, is_active, expires_at) VALUES
    ('Joj04SbKXv1KYAIgc+bNkAF2JuVsPzCxlNU+Ypnt/jw=', 'Test API Key', 'Тестовый API ключ для демонстрации функционала', true, NULL)
ON CONFLICT (key_hash) DO NOTHING;

