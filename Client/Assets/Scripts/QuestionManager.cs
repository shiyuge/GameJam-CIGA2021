using System;
using System.Collections.Generic;

public class QuestionManager
{
    private Random _random;
    private List<List<Question>> _pool;
    private HashSet<Question> _visited;
    private int _index;

    private Question getQuestion()
    {
        List<Question> candidates = _pool[_index];
        while (candidates.Count != 0)
        {
            int i = _random.Next(0, candidates.Count);
            Question q = candidates[i];
            if (!_visited.Contains(q))
            {
                return q;
            }

            candidates.Remove(q);
        }
        return null;
    }
    
    private Question _current;
    
    public void MoveToNextQuestion()
    {
        _visited.Add(_current);
        _index += 1;
        _current = getQuestion();
    }
    
    public Question CurrentQuestion()
    {
        return _current;
    }

    public QuestionManager()
    {
        _pool = Config.QuestionPool;
        _visited = new HashSet<Question>();
        _random = new Random();

        _index = 0;
        _current = getQuestion();
    }
}
