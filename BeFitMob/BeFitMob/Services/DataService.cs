using System;
using System.Collections.Generic;
using System.Linq;
using BeFitMob.Models;

namespace BeFitMob.Services;

public class DataService
{
    private readonly List<ExerciseType> _exerciseTypes = new();
    private readonly List<TrainingSession> _sessions = new();
    private readonly List<PerformedExercise> _exercises = new();

    private int _nextExerciseTypeId = 1;
    private int _nextSessionId = 1;
    private int _nextExerciseId = 1;

    public DataService()
    {
        SeedExerciseTypes();
    }

    private void SeedExerciseTypes()
    {
        if (_exerciseTypes.Any())
            return;

        AddExerciseType(new ExerciseType
        {
            Name = "Przysiad ze sztangą",
            Description = "Ćwiczenie na nogi i pośladki."
        });

        AddExerciseType(new ExerciseType
        {
            Name = "Wyciskanie na ławce",
            Description = "Ćwiczenie na klatkę piersiową."
        });

        AddExerciseType(new ExerciseType
        {
            Name = "Martwy ciąg",
            Description = "Ćwiczenie ogólnorozwojowe."
        });
    }

    // ----- TYPY ĆWICZEŃ -----

    public IReadOnlyList<ExerciseType> GetExerciseTypes()
        => _exerciseTypes.OrderBy(e => e.Name).ToList();

    public ExerciseType? GetExerciseTypeById(int id)
        => _exerciseTypes.FirstOrDefault(e => e.Id == id);

    public ExerciseType AddExerciseType(ExerciseType type)
    {
        type.Id = _nextExerciseTypeId++;
        _exerciseTypes.Add(type);
        return type;
    }

    public bool UpdateExerciseType(ExerciseType type)
    {
        var existing = GetExerciseTypeById(type.Id);
        if (existing == null)
            return false;

        existing.Name = type.Name;
        existing.Description = type.Description;
        return true;
    }

    public bool DeleteExerciseType(int id)
    {
        var existing = GetExerciseTypeById(id);
        if (existing == null)
            return false;

        // Usuwamy także ćwiczenia tego typu
        _exercises.RemoveAll(e => e.ExerciseTypeId == id);

        _exerciseTypes.Remove(existing);
        return true;
    }

    // ----- SESJE TRENINGOWE (związane z użytkownikiem) -----

    public IReadOnlyList<TrainingSession> GetSessionsForUser(string userId)
        => _sessions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.StartTime)
            .ToList();

    public TrainingSession? GetSessionForUserById(int id, string userId)
        => _sessions.FirstOrDefault(s => s.Id == id && s.UserId == userId);

    public TrainingSession AddSession(TrainingSession session, string userId)
    {
        session.Id = _nextSessionId++;
        session.UserId = userId;
        _sessions.Add(session);
        return session;
    }

    public bool UpdateSession(TrainingSession session, string userId)
    {
        var existing = GetSessionForUserById(session.Id, userId);
        if (existing == null)
            return false;

        existing.StartTime = session.StartTime;
        existing.EndTime = session.EndTime;
        return true;
    }

    public bool DeleteSession(int id, string userId)
    {
        var existing = GetSessionForUserById(id, userId);
        if (existing == null)
            return false;

        // Usuń ćwiczenia z tej sesji
        _exercises.RemoveAll(e => e.TrainingSessionId == id);

        _sessions.Remove(existing);
        return true;
    }

    // ----- WYKONANE ĆWICZENIA (tylko własne sesje) -----

    public IReadOnlyList<PerformedExercise> GetExercisesForUser(string userId)
    {
        var userSessionIds = _sessions
            .Where(s => s.UserId == userId)
            .Select(s => s.Id)
            .ToHashSet();

        var sessionsDict = _sessions.ToDictionary(s => s.Id);
        var typesDict = _exerciseTypes.ToDictionary(t => t.Id);

        var list = _exercises
            .Where(e => userSessionIds.Contains(e.TrainingSessionId))
            .ToList();

        foreach (var e in list)
        {
            if (sessionsDict.TryGetValue(e.TrainingSessionId, out var sess))
                e.TrainingSession = sess;
            else
                e.TrainingSession = null;

            if (typesDict.TryGetValue(e.ExerciseTypeId, out var type))
                e.ExerciseType = type;
            else
                e.ExerciseType = null;
        }

        return list
            .OrderByDescending(e => e.TrainingSession!.StartTime)
            .ToList();
    }

    public PerformedExercise? GetExerciseForUserById(int id, string userId)
    {
        var ex = _exercises.FirstOrDefault(e => e.Id == id);
        if (ex == null)
            return null;

        var session = _sessions.FirstOrDefault(s => s.Id == ex.TrainingSessionId && s.UserId == userId);
        if (session == null)
            return null;

        ex.TrainingSession = session;
        ex.ExerciseType = _exerciseTypes.FirstOrDefault(t => t.Id == ex.ExerciseTypeId);
        return ex;
    }

    public PerformedExercise? AddExercise(PerformedExercise exercise, string userId)
    {
        var session = GetSessionForUserById(exercise.TrainingSessionId, userId);
        if (session == null)
            return null;

        exercise.Id = _nextExerciseId++;
        exercise.TrainingSession = session;
        exercise.ExerciseType = _exerciseTypes.FirstOrDefault(t => t.Id == exercise.ExerciseTypeId);
        _exercises.Add(exercise);
        return exercise;
    }

    public bool UpdateExercise(PerformedExercise exercise, string userId)
    {
        var existing = _exercises.FirstOrDefault(e => e.Id == exercise.Id);
        if (existing == null)
            return false;

        var session = GetSessionForUserById(exercise.TrainingSessionId, userId);
        if (session == null)
            return false;

        existing.TrainingSessionId = exercise.TrainingSessionId;
        existing.ExerciseTypeId = exercise.ExerciseTypeId;
        existing.Weight = exercise.Weight;
        existing.Sets = exercise.Sets;
        existing.Reps = exercise.Reps;
        existing.Notes = exercise.Notes;

        existing.TrainingSession = session;
        existing.ExerciseType = _exerciseTypes.FirstOrDefault(t => t.Id == exercise.ExerciseTypeId);

        return true;
    }

    public bool DeleteExercise(int id, string userId)
    {
        var existing = _exercises.FirstOrDefault(e => e.Id == id);
        if (existing == null)
            return false;

        var session = GetSessionForUserById(existing.TrainingSessionId, userId);
        if (session == null)
            return false;

        _exercises.Remove(existing);
        return true;
    }

    // ----- STATYSTYKI -----

    public IReadOnlyList<ExerciseStats> GetStatsForUser(string userId)
    {
        var cutoff = DateTime.Now.AddDays(-28);

        var userSessionIds = _sessions
            .Where(s => s.UserId == userId && s.StartTime >= cutoff)
            .Select(s => s.Id)
            .ToHashSet();

        var relevantExercises = _exercises
            .Where(e => userSessionIds.Contains(e.TrainingSessionId))
            .ToList();

        var typesDict = _exerciseTypes.ToDictionary(t => t.Id);

        var result = relevantExercises
            .GroupBy(e => e.ExerciseTypeId)
            .Select(g =>
            {
                var typeName = typesDict.TryGetValue(g.Key, out var type)
                    ? type.Name
                    : "Nieznane ćwiczenie";

                return new ExerciseStats
                {
                    ExerciseName = typeName,
                    TimesPerformed = g.Count(),
                    TotalReps = g.Sum(x => x.Sets * x.Reps),
                    AverageWeight = g.Average(x => x.Weight),
                    MaxWeight = g.Max(x => x.Weight)
                };
            })
            .OrderBy(s => s.ExerciseName)
            .ToList();

        return result;
    }
}