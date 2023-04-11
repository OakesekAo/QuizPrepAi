My general feedback for using ChatGPT to build an app to help with quizzing you on a particular topic you select:

TLDR: For high-level and specific things, it can do well. For complex things, it will lead you down rabbit holes, and you can easily get lost.

Starting out, I had a couple of main goals with this project:

1. On the idea, I wanted a real-world SaaS product. Doing a quiz style app is of a few dozen ideas from ChatGPT that was actually on the smaller side for a real product. Most weren't viable ideas, even with unlimited time.

2. Get an MVP (minimum viable product) quickly and iterate from there.

So with the idea in hand, my basic premise was to get a quiz, grade it, and put it in a DB. Which is about as far as I got.

I did use ASP.NET with MVC and Postgres for this.

I started off scaffolding a basic ASP.net project out, no problem. I decided early I might as well get it published and live asap and then go from there. That was done quickly enough.

Next, I was trying to get ChatGPT to give me a workable API call, cause why not? It was unable to give me anything that worked without major finessing. I ended up basically rebuilding the basic HTTP request and everything. Though, in ChatGPT's defense, it was more due to me not understanding the JSON response structure more than anything.

I did end up getting it working eventually and then attempted to move to one of the .NET OpenAI libraries. Between these two steps, I sank the majority of my time as I couldn't figure out why the library wasn't working. The main issue was that some of the parameters that were used with the library had either incorrect implementation or documentation, or something along those lines. Eventually, I figured out the issue and took that parameter out of the equation, and it finally worked.

All of this was going on with me asking rubber duck ChatGPT, and he would lead me down rabbit holes and blow up model classes with all kinds of random stuff. It really took the steam out of the project.

As a final major hurdle, I was at the point where I was trying to loop through quiz questions with JS controlling it on a single view (no additional post), and I couldn't (or ChatGPT couldn't) figure out the loop spaghetti. I should have scrapped it sooner as I spent to much time here.

I eventually transitioned to a single question per view, followed by the results at the end. I had to majorly clean up and restructure my quiz/question/answer model relations after I allowed ChatGPT to blow it up a few times.

At this point, the MVP was done, and time was running short, so I slapped on a kind of minimal BS5 lander and styling.

So a few things ChatGPT was "good" at was helping me sort out the parsing on the response. I pasted the unedited string, told it how I wanted it chopped up, and it spit out some decent parsing. It likely saved me a few hours muddling through some splits and a few other things. Now, trying to factor in some conditions and having ChatGPT integrate that made it unusable, so, again, it has its useful limits. It goes back to you could likely do that precise thing faster than prompting it to a solution.

So would I do it the same way again? Not in the same fashion. I intentionally tried to get it to generate a lot of the code. Trying to get useful code takes a lot of finessing.

There is still things that helped quite a bit, like certain ideas on how to refactor stuff or explaining code and adjusting things that I've made and am stumped on in a very focused way. This can help move things along more quickly than digging through just documentation, stack overflow, or google and still being stuck due to not fully understanding. It may not be fully correct most of the time, but it does a fairly decent job of leaving breadcrumbs to help you understand to move forward if you can remember to refocus yourself on what your doing.
