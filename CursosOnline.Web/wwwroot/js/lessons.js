// lessons.js

let currentLessons = [];

async function loadLessons(courseId) {
    try {
        const response = await fetch(`/api/courses/${courseId}`, {
            headers: getAuthHeaders()
        });

        if (response.ok) {
            const course = await response.json();
            currentLessons = (course.lessons || []).sort((a, b) => a.order - b.order);
            renderLessonsTable(currentLessons);
        }
    } catch (error) {
        console.error('Error loading lessons:', error);
    }
}

async function saveLesson() {
    const courseId = document.getElementById('lessonCourseId').value;
    const lessonId = document.getElementById('lessonId').value;
    const title = document.getElementById('lessonTitle').value;
    const order = parseInt(document.getElementById('lessonOrder').value);

    const lessonData = {
        courseId: courseId,
        title: title,
        order: order
    };

    try {
        let response;
        if (lessonId) {
            response = await fetch(`/api/lessons/${lessonId}`, {
                method: 'PUT',
                headers: getAuthHeaders(),
                body: JSON.stringify(lessonData)
            });
        } else {
            response = await fetch('/api/lessons', {
                method: 'POST',
                headers: getAuthHeaders(),
                body: JSON.stringify(lessonData)
            });
        }

        if (response.ok || response.status === 204) {
            bootstrap.Modal.getInstance(document.getElementById('lessonModal')).hide();
            loadCourseDetails(courseId);
        } else {
            const error = await response.json();
            alert(error.message || 'Failed to save lesson');
        }
    } catch (error) {
        console.error('Error saving lesson:', error);
        alert('An error occurred');
    }
}

async function deleteLesson(id) {
    if (!confirm('Are you sure you want to delete this lesson?')) {
        return;
    }

    try {
        const response = await fetch(`/api/lessons/${id}`, {
            method: 'DELETE',
            headers: getAuthHeaders()
        });

        if (response.ok || response.status === 204) {
            const courseId = document.getElementById('lessonCourseId').value;
            loadCourseDetails(courseId);
        } else {
            alert('Failed to delete lesson');
        }
    } catch (error) {
        console.error('Error deleting lesson:', error);
        alert('An error occurred');
    }
}

async function moveLessonUp(index) {
    if (index === 0) return;

    const lesson = currentLessons[index];
    const previousLesson = currentLessons[index - 1];

    // Swap orders
    const tempOrder = lesson.order;
    lesson.order = previousLesson.order;
    previousLesson.order = tempOrder;

    await updateLessonOrder(lesson);
    await updateLessonOrder(previousLesson);

    const courseId = document.getElementById('lessonCourseId').value;
    loadCourseDetails(courseId);
}

async function moveLessonDown(index) {
    if (index === currentLessons.length - 1) return;

    const lesson = currentLessons[index];
    const nextLesson = currentLessons[index + 1];

    // Swap orders
    const tempOrder = lesson.order;
    lesson.order = nextLesson.order;
    nextLesson.order = tempOrder;

    await updateLessonOrder(lesson);
    await updateLessonOrder(nextLesson);

    const courseId = document.getElementById('lessonCourseId').value;
    loadCourseDetails(courseId);
}

async function updateLessonOrder(lesson) {
    try {
        const response = await fetch(`/api/lessons/${lesson.id}`, {
            method: 'PUT',
            headers: getAuthHeaders(),
            body: JSON.stringify({
                courseId: lesson.courseId,
                title: lesson.title,
                order: lesson.order
            })
        });

        if (!response.ok && response.status !== 204) {
            throw new Error('Failed to update lesson order');
        }
    } catch (error) {
        console.error('Error updating lesson order:', error);
        throw error;
    }
}
