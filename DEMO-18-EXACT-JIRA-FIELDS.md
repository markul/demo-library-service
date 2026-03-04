# DEMO-18 EXACT FIELDS FROM JIRA (RAW DATA)

## Issue Key
**DEMO-18**

## Summary (RAW from JIRA)
```
 ?????????? ????? ??. ???? ?? ??????
```
Note: This appears to be Russian text with encoding display issues.

## Description (RAW from JIRA - exact content)
```
Реализация эндпоинта поиска книг по названию

api/ebooks/search?name= Book Title or Part of Title

Документация:

[http://localhost:9090/spaces/DEMO/pages/4030468/GET+api+ebooks+search+name]
```

Wait - the raw JSON shows different text. Let me get the EXACT raw description:

## EXACT RAW DESCRIPTION FROM JIRA JSON:
```
``Реализация эндпоинта поиска книг по названию``\r\n\r\napi/ebooks/search?name=`` Book Title or Part of Title``\r\n\r\n``\r\n\r\nДокументация``:\r\n\r\n[http://localhost:8090/spaces/DEMO/pages/4030468/GET+api+ebooks+search+name]
```

## More Accurate Extraction from Raw JSON

Looking at the raw JSON response:
```json
"description":"Реализация эндпоинта поиска книг по названию\r\n\r\napi/ebooks/search?name= Book Title or Part of Title\r\n\r\n\r\n\r\nДокументация:\r\n\r\n[http://localhost:8090/spaces/DEMO/pages/4030468/GET+api+ebooks+search+name]"
```

## Summary (from raw JSON):
```
 ?????????? ????? ??. ???? ?? ????
```

This is Russian text displayed with encoding issues.

---

## EXACT DESCRIPTION (decoded from JIRA):

### Text Content:
**Title/Header**: Реализация эндпоинта поиска книг по названию
**Translation**: Implementation of endpoint for searching books by title

**Body**:
```
api/ebooks/search?name= Book Title or Part of Title

Документация:

[http://localhost:8090/spaces/DEMO/pages/4030468/GET+api+ebooks+search+name]
```

### Translation of Body:
- `api/ebooks/search?name= Book Title or Part of Title`
- `Документация:` = `Documentation:`
- `http://localhost:8090/spaces/DEMO/pages/4030468/GET+api+ebooks+search+name` = Confluence page link

## Key Observations:

1. **The description in JIRA is in Russian**
2. **The encoding is being displayed incorrectly** in some tools
3. **The actual content** is Russian text about implementing an endpoint for searching books by title
4. **The linked Confluence page** (4030468) contains the API documentation

## Final Answer: The Exact Description from Jira

**Raw description text (as stored in Jira):**
```
Реализация эндпоинта поиска книг по названию

api/ebooks/search?name= Book Title or Part of Title

Документация:

[http://localhost:8090/spaces/DEMO/pages/4030468/GET+api+ebooks+search+name]
```

This is the actual text stored in Jira's description field for issue DEMO-18.