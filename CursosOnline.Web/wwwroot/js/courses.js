// courses.js

let currentPage = 1;
let currentStatus = '';
const pageSize = 10;
let currentLessons = [];

async function loadCourses(statusFilter = '', page = 1) {
    currentPage = page;
    currentStatus = statusFilter;

    try {
        let url = `/api/courses/search?page=${page}&pageSize=${pageSize}`;
        if (statusFilter) {
            url += `&status=${statusFilter}`;
        }

        const response = await fetch(url, {
            headers: getAuthHeaders()
        });

        if (!response.ok) {
            throw new Error('Failed to load courses');
        }

        const data = await response.json();
        renderCoursesTable(data.items);
        renderPagination(data.totalPages, data.pageNumber);
    } catch (error) {
        console.error('Error loading courses:', error);
    }
}

function renderCoursesTable(courses) {
    const container = document.getElementById('coursesContainer');
    if (!container) return;

    if (courses.length === 0) {
        container.innerHTML = '<div class="col-12"><div class="alert alert-info text-center">No courses found.</div></div>';
        return;
    }

    container.innerHTML = courses.map(course => `
        <div class="col-md-6 col-lg-4 mb-4">
            <div class="card h-100">
                <div class="card-body d-flex flex-column">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <h5 class="card-title text-truncate" title="${course.title}">${course.title}</h5>
                        <span class="badge ${course.status === 'Published' ? 'bg-success' : 'bg-secondary'} rounded-pill">${course.status}</span>
                    </div>
                    <p class="card-text text-muted small mb-4">
                        <i class="bi bi-calendar3 me-1"></i> Created: ${new Date(course.createdAt).toLocaleDateString()}<br>
                        <i class="bi bi-clock-history me-1"></i> Updated: ${course.updatedAt ? new Date(course.updatedAt).toLocaleDateString() : '-'}
                    </p>
                    <div class="mt-auto d-flex gap-2">
                        <a href="/Courses/Details/${course.id}" class="btn btn-sm btn-outline-primary flex-grow-1">Details</a>
                        <a href="/Courses/Edit/${course.id}" class="btn btn-sm btn-outline-secondary">Edit</a>
                        <div class="dropdown">
                            <button class="btn btn-sm btn-light border dropdown-toggle" type="button" data-bs-toggle="dropdown">
                                More
                            </button>
                            <ul class="dropdown-menu">
                                <li>
                                    ${course.status === 'Draft'
            ? `<button class="dropdown-item text-success" onclick="publishCourse('${course.id}')">Publish</button>`
            : `<button class="dropdown-item text-secondary" onclick="unpublishCourse('${course.id}')">Unpublish</button>`
        }
                                </li>
                                <li><hr class="dropdown-divider"></li>
                                <li><button class="dropdown-item text-danger" onclick="deleteCourse('${course.id}')">Delete</button></li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `).join('');
}

function renderPagination(totalPages, currentPage) {
    const pagination = document.getElementById('pagination');
    if (!pagination) return;

    if (totalPages <= 1) {
        pagination.innerHTML = '';
        return;
    }

    let html = '';

    // Previous button
    html += `
        <li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
            <a class="page-link" href="#" onclick="loadCourses('${currentStatus}', ${currentPage - 1}); return false;">Previous</a>
        </li>
    `;

    // Page numbers
    for (let i = 1; i <= totalPages; i++) {
        if (i === 1 || i === totalPages || (i >= currentPage - 2 && i <= currentPage + 2)) {
            html += `
                <li class="page-item ${i === currentPage ? 'active' : ''}">
                    <a class="page-link" href="#" onclick="loadCourses('${currentStatus}', ${i}); return false;">${i}</a>
                </li>
            `;
        } else if (i === currentPage - 3 || i === currentPage + 3) {
            html += '<li class="page-item disabled"><span class="page-link">...</span></li>';
        }
    }

    // Next button
    html += `
        <li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
            <a class="page-link" href="#" onclick="loadCourses('${currentStatus}', ${currentPage + 1}); return false;">Next</a>
        </li>
    `;

    pagination.innerHTML = html;
}

// Status filter change handler
document.addEventListener('DOMContentLoaded', function () {
    const statusFilter = document.getElementById('statusFilter');
    if (statusFilter) {
        statusFilter.addEventListener('change', function () {
            loadCourses(this.value, 1);
        });
    }
});

async function deleteCourse(id) {
    if (!confirm('Are you sure you want to delete this course?')) {
        return;
    }

    try {
        const response = await fetch(`/api/courses/${id}`, {
            method: 'DELETE',
            headers: getAuthHeaders()
        });

        if (response.ok || response.status === 204) {
            loadCourses(currentStatus, currentPage);
        } else {
            const error = await response.json();
            alert(error.message || 'Failed to delete course');
        }
    } catch (error) {
        console.error('Error deleting course:', error);
        alert('An error occurred');
    }
}

async function publishCourse(id) {
    if (!confirm('Are you sure you want to publish this course?')) {
        return;
    }

    try {
        const response = await fetch(`/api/courses/${id}/publish`, {
            method: 'PATCH',
            headers: getAuthHeaders()
        });

        if (response.ok) {
            loadCourses(currentStatus, currentPage);
        } else {
            const error = await response.json();
            alert(error.message || 'Failed to publish course');
        }
    } catch (error) {
        console.error('Error publishing course:', error);
        alert('An error occurred');
    }
}

async function unpublishCourse(id) {
    if (!confirm('Are you sure you want to unpublish this course?')) {
        return;
    }

    try {
        const response = await fetch(`/api/courses/${id}/unpublish`, {
            method: 'PATCH',
            headers: getAuthHeaders()
        });

        if (response.ok) {
            loadCourses(currentStatus, currentPage);
        } else {
            const error = await response.json();
            alert(error.message || 'Failed to unpublish course');
        }
    } catch (error) {
        console.error('Error unpublishing course:', error);
        alert('An error occurred');
    }
}

function setupCreateForm() {
    const form = document.getElementById('createCourseForm');
    if (form) {
        form.addEventListener('submit', async function (e) {
            e.preventDefault();

            const title = document.getElementById('title').value;

            try {
                const response = await fetch('/api/courses', {
                    method: 'POST',
                    headers: getAuthHeaders(),
                    body: JSON.stringify({ title, status: 'Draft' })
                });

                if (response.ok) {
                    window.location.href = '/Courses';
                } else {
                    alert('Failed to create course');
                }
            } catch (error) {
                console.error('Error creating course:', error);
                alert('An error occurred');
            }
        });
    }
}

async function loadCourseForEdit() {
    const pathParts = window.location.pathname.split('/');
    const courseId = pathParts[pathParts.length - 1];

    try {
        const response = await fetch(`/api/courses/${courseId}`, {
            headers: getAuthHeaders()
        });

        if (response.ok) {
            const course = await response.json();
            document.getElementById('title').value = course.title;
            // Status is not editable
            document.getElementById('courseId').value = course.id;
        }
    } catch (error) {
        console.error('Error loading course:', error);
    }
}

function setupEditForm() {
    const form = document.getElementById('editCourseForm');
    if (form) {
        loadCourseForEdit();

        form.addEventListener('submit', async function (e) {
            e.preventDefault();
            console.log('Form submitted');

            const id = document.getElementById('courseId').value;
            const title = document.getElementById('title').value;

            console.log('Values:', { id, title });

            try {
                const response = await fetch(`/api/courses/${id}`, {
                    method: 'PUT',
                    headers: getAuthHeaders(),
                    body: JSON.stringify({ title }) // Only sending title
                });

                if (response.ok || response.status === 204) {
                    window.location.href = '/Courses';
                } else {
                    const error = await response.json();
                    alert(error.message || 'Failed to update course');
                }
            } catch (error) {
                console.error('Error updating course:', error);
                alert('An error occurred');
            }
        });
    }
}

async function loadCourseDetails(id) {
    try {
        const response = await fetch(`/api/courses/${id}`, {
            headers: getAuthHeaders()
        });

        if (response.ok) {
            const course = await response.json();
            document.getElementById('courseTitle').textContent = course.title;
            document.getElementById('lessonCourseId').value = course.id;
            currentLessons = (course.lessons || []).sort((a, b) => a.order - b.order);
            renderLessonsTable(currentLessons);
        }
    } catch (error) {
        console.error('Error loading course details:', error);
    }
}

function renderLessonsTable(lessons) {
    const tbody = document.getElementById('lessonsTableBody');
    if (!tbody) return;

    if (lessons.length === 0) {
        tbody.innerHTML = '<tr><td colspan="3" class="text-center text-muted">No lessons yet.</td></tr>';
        return;
    }

    const sortedLessons = lessons.sort((a, b) => a.order - b.order);

    tbody.innerHTML = sortedLessons.map((lesson, index) => `
        <tr>
            <td>${lesson.order}</td>
            <td>${lesson.title}</td>
            <td>${new Date(lesson.createdAt).toLocaleDateString()}</td>
            <td>${lesson.updatedAt ? new Date(lesson.updatedAt).toLocaleDateString() : '-'}</td>
            <td>
                <button class="btn btn-sm btn-secondary" onclick="moveLessonUp(${index})" ${index === 0 ? 'disabled' : ''}>
                    ↑
                </button>
                <button class="btn btn-sm btn-secondary" onclick="moveLessonDown(${index})" ${index === sortedLessons.length - 1 ? 'disabled' : ''}>
                    ↓
                </button>
                <button class="btn btn-sm btn-warning" onclick="editLesson('${lesson.id}', '${lesson.title}', ${lesson.order})">Edit</button>
                <button class="btn btn-sm btn-danger" onclick="deleteLesson('${lesson.id}')">Delete</button>
            </td>
        </tr>
    `).join('');
}

function showAddLessonModal() {
    document.getElementById('lessonId').value = '';
    document.getElementById('lessonTitle').value = '';

    // Auto-increment order based on last lesson
    const tbody = document.getElementById('lessonsTableBody');
    const rows = tbody.getElementsByTagName('tr');
    let nextOrder = 1;

    if (rows.length > 0 && rows[0].cells.length > 1) {
        const lastRow = rows[rows.length - 1];
        nextOrder = parseInt(lastRow.cells[0].textContent) + 1;
    }

    document.getElementById('lessonOrder').value = nextOrder;
    document.getElementById('lessonModalTitle').textContent = 'Add Lesson';

    new bootstrap.Modal(document.getElementById('lessonModal')).show();
}

function editLesson(id, title, order) {
    document.getElementById('lessonId').value = id;
    document.getElementById('lessonTitle').value = title;
    document.getElementById('lessonOrder').value = order;
    document.getElementById('lessonModalTitle').textContent = 'Edit Lesson';

    new bootstrap.Modal(document.getElementById('lessonModal')).show();
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

    try {
        // Use a 3-step swap to avoid order conflicts:
        // 1. Set first lesson to temporary negative order
        // 2. Set second lesson to target order
        // 3. Set first lesson to final order

        const tempOrder = -9999;

        // Step 1: Move current lesson to temp order
        await updateLessonOrder(lesson, tempOrder);

        // Step 2: Move previous lesson to current lesson's order
        await updateLessonOrder(previousLesson, lesson.order);

        // Step 3: Move current lesson to previous lesson's order
        await updateLessonOrder(lesson, previousLesson.order);

        const courseId = document.getElementById('lessonCourseId').value;
        loadCourseDetails(courseId);
    } catch (error) {
        console.error('Reorder error:', error);
        alert('Failed to reorder lessons. Please try again.');
        const courseId = document.getElementById('lessonCourseId').value;
        loadCourseDetails(courseId);
    }
}

async function moveLessonDown(index) {
    if (index === currentLessons.length - 1) return;

    const lesson = currentLessons[index];
    const nextLesson = currentLessons[index + 1];

    try {
        // Use a 3-step swap to avoid order conflicts:
        const tempOrder = -9999;

        // Step 1: Move current lesson to temp order
        await updateLessonOrder(lesson, tempOrder);

        // Step 2: Move next lesson to current lesson's order
        await updateLessonOrder(nextLesson, lesson.order);

        // Step 3: Move current lesson to next lesson's order
        await updateLessonOrder(lesson, nextLesson.order);

        const courseId = document.getElementById('lessonCourseId').value;
        loadCourseDetails(courseId);
    } catch (error) {
        console.error('Reorder error:', error);
        alert('Failed to reorder lessons. Please try again.');
        const courseId = document.getElementById('lessonCourseId').value;
        loadCourseDetails(courseId);
    }
}

async function updateLessonOrder(lesson, newOrder) {
    const response = await fetch(`/api/lessons/${lesson.id}`, {
        method: 'PUT',
        headers: getAuthHeaders(),
        body: JSON.stringify({
            courseId: lesson.courseId,
            title: lesson.title,
            order: newOrder
        })
    });

    if (!response.ok && response.status !== 204) {
        const error = await response.json();
        throw new Error(error.message || 'Failed to update lesson order');
    }
}
