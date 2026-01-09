UPDATE locations SET 
    name = CASE id
        WHEN (SELECT id FROM locations WHERE name = 'Eiffel Tower' LIMIT 1) THEN 'Эйфелева башня'
        WHEN (SELECT id FROM locations WHERE name = 'Big Ben' LIMIT 1) THEN 'Биг-Бен'
        WHEN (SELECT id FROM locations WHERE name = 'Colosseum' LIMIT 1) THEN 'Колизей'
        WHEN (SELECT id FROM locations WHERE name = 'Sagrada Familia' LIMIT 1) THEN 'Саграда Фамилия'
        WHEN (SELECT id FROM locations WHERE name = 'Acropolis' LIMIT 1) THEN 'Акрополь'
        ELSE name
    END,
    country = CASE id
        WHEN (SELECT id FROM locations WHERE name = 'Eiffel Tower' OR name = 'Эйфелева башня' LIMIT 1) THEN 'Франция'
        WHEN (SELECT id FROM locations WHERE name = 'Big Ben' OR name = 'Биг-Бен' LIMIT 1) THEN 'Великобритания'
        WHEN (SELECT id FROM locations WHERE name = 'Colosseum' OR name = 'Колизей' LIMIT 1) THEN 'Италия'
        WHEN (SELECT id FROM locations WHERE name = 'Sagrada Familia' OR name = 'Саграда Фамилия' LIMIT 1) THEN 'Испания'
        WHEN (SELECT id FROM locations WHERE name = 'Acropolis' OR name = 'Акрополь' LIMIT 1) THEN 'Греция'
        ELSE country
    END,
    city = CASE id
        WHEN (SELECT id FROM locations WHERE name = 'Eiffel Tower' OR name = 'Эйфелева башня' LIMIT 1) THEN 'Париж'
        WHEN (SELECT id FROM locations WHERE name = 'Big Ben' OR name = 'Биг-Бен' LIMIT 1) THEN 'Лондон'
        WHEN (SELECT id FROM locations WHERE name = 'Colosseum' OR name = 'Колизей' LIMIT 1) THEN 'Рим'
        WHEN (SELECT id FROM locations WHERE name = 'Sagrada Familia' OR name = 'Саграда Фамилия' LIMIT 1) THEN 'Барселона'
        WHEN (SELECT id FROM locations WHERE name = 'Acropolis' OR name = 'Акрополь' LIMIT 1) THEN 'Афины'
        ELSE city
    END,
    description = CASE id
        WHEN (SELECT id FROM locations WHERE name = 'Eiffel Tower' OR name = 'Эйфелева башня' LIMIT 1) THEN 'Знаменитая железная башня'
        WHEN (SELECT id FROM locations WHERE name = 'Big Ben' OR name = 'Биг-Бен' LIMIT 1) THEN 'Историческая часовая башня'
        WHEN (SELECT id FROM locations WHERE name = 'Colosseum' OR name = 'Колизей' LIMIT 1) THEN 'Древний амфитеатр'
        WHEN (SELECT id FROM locations WHERE name = 'Sagrada Familia' OR name = 'Саграда Фамилия' LIMIT 1) THEN 'Незавершенная базилика'
        WHEN (SELECT id FROM locations WHERE name = 'Acropolis' OR name = 'Акрополь' LIMIT 1) THEN 'Древняя цитадель'
        ELSE description
    END
WHERE name IN ('Eiffel Tower', 'Big Ben', 'Colosseum', 'Sagrada Familia', 'Acropolis');

UPDATE tags SET 
    name = CASE name
        WHEN 'Adventure' THEN 'Приключения'
        WHEN 'Relaxation' THEN 'Отдых'
        WHEN 'Culture' THEN 'Культура'
        WHEN 'Nature' THEN 'Природа'
        WHEN 'Food' THEN 'Еда'
        WHEN 'History' THEN 'История'
        ELSE name
    END
WHERE name IN ('Adventure', 'Relaxation', 'Culture', 'Nature', 'Food', 'History');

