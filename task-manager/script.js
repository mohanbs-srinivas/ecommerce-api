/**
 * Task Manager Application
 * A comprehensive task management system with local storage persistence
 * 
 * Features:
 * - CRUD operations for tasks
 * - Local storage persistence
 * - Filtering and statistics
 * - Import/export functionality
 * - Comprehensive error handling
 * - Keyboard shortcuts
 * - Accessibility support
 */

// ===== UTILITY FUNCTIONS =====
class Utils {
    /**
     * Generate a unique ID for tasks
     */
    static generateId() {
        return 'task_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
    }

    /**
     * Sanitize HTML content to prevent XSS attacks
     */
    static sanitizeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    /**
     * Format date for display
     */
    static formatDate(dateString) {
        const date = new Date(dateString);
        const now = new Date();
        const diffMs = now - date;
        const diffMins = Math.floor(diffMs / 60000);
        const diffHours = Math.floor(diffMs / 3600000);
        const diffDays = Math.floor(diffMs / 86400000);

        if (diffMins < 1) return 'Just now';
        if (diffMins < 60) return `${diffMins}m ago`;
        if (diffHours < 24) return `${diffHours}h ago`;
        if (diffDays < 7) return `${diffDays}d ago`;
        
        return date.toLocaleDateString();
    }

    /**
     * Debounce function for performance optimization
     */
    static debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func.apply(this, args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }

    /**
     * Validate task text
     */
    static validateTaskText(text) {
        if (!text || typeof text !== 'string') {
            return { valid: false, error: 'Task text is required' };
        }
        
        const trimmed = text.trim();
        if (trimmed.length === 0) {
            return { valid: false, error: 'Task text cannot be empty' };
        }
        
        if (trimmed.length > 500) {
            return { valid: false, error: 'Task text cannot exceed 500 characters' };
        }
        
        return { valid: true, text: trimmed };
    }
}

// ===== LOGGER CLASS =====
class Logger {
    static levels = {
        ERROR: 0,
        WARN: 1,
        INFO: 2,
        DEBUG: 3
    };

    static currentLevel = Logger.levels.INFO;

    static log(level, message, data = null) {
        if (level <= Logger.currentLevel) {
            const timestamp = new Date().toISOString();
            const levelName = Object.keys(Logger.levels)[level];
            const logMessage = `[${timestamp}] ${levelName}: ${message}`;
            
            console.log(logMessage, data || '');
            
            // Store logs for debugging (keep last 100 entries)
            if (!window.taskManagerLogs) window.taskManagerLogs = [];
            window.taskManagerLogs.push({ timestamp, level: levelName, message, data });
            if (window.taskManagerLogs.length > 100) {
                window.taskManagerLogs.shift();
            }
        }
    }

    static error(message, data) { Logger.log(Logger.levels.ERROR, message, data); }
    static warn(message, data) { Logger.log(Logger.levels.WARN, message, data); }
    static info(message, data) { Logger.log(Logger.levels.INFO, message, data); }
    static debug(message, data) { Logger.log(Logger.levels.DEBUG, message, data); }
}

// ===== STORAGE MANAGER =====
class StorageManager {
    static STORAGE_KEY = 'taskManagerData';
    static STORAGE_VERSION = '1.0';

    /**
     * Save tasks to localStorage with error handling
     */
    static saveTasks(tasks) {
        try {
            const data = {
                version: StorageManager.STORAGE_VERSION,
                timestamp: new Date().toISOString(),
                tasks: tasks
            };
            
            localStorage.setItem(StorageManager.STORAGE_KEY, JSON.stringify(data));
            Logger.info('Tasks saved to localStorage', { count: tasks.length });
            return true;
        } catch (error) {
            Logger.error('Failed to save tasks to localStorage', error);
            throw new Error('Failed to save tasks. Your browser may not support localStorage or storage is full.');
        }
    }

    /**
     * Load tasks from localStorage with validation
     */
    static loadTasks() {
        try {
            const stored = localStorage.getItem(StorageManager.STORAGE_KEY);
            if (!stored) {
                Logger.info('No stored tasks found');
                return [];
            }

            const data = JSON.parse(stored);
            
            // Validate data structure
            if (!data.tasks || !Array.isArray(data.tasks)) {
                Logger.warn('Invalid stored data structure, returning empty array');
                return [];
            }

            // Validate and filter tasks
            const validTasks = data.tasks.filter(task => StorageManager.validateTask(task));
            
            Logger.info('Tasks loaded from localStorage', { 
                total: data.tasks.length, 
                valid: validTasks.length 
            });
            
            return validTasks;
        } catch (error) {
            Logger.error('Failed to load tasks from localStorage', error);
            return [];
        }
    }

    /**
     * Validate individual task structure
     */
    static validateTask(task) {
        return task &&
               typeof task.id === 'string' &&
               typeof task.text === 'string' &&
               typeof task.completed === 'boolean' &&
               typeof task.createdAt === 'string' &&
               typeof task.updatedAt === 'string' &&
               task.text.trim().length > 0;
    }

    /**
     * Check localStorage availability
     */
    static isAvailable() {
        try {
            const test = '__localStorage_test__';
            localStorage.setItem(test, test);
            localStorage.removeItem(test);
            return true;
        } catch {
            return false;
        }
    }

    /**
     * Get storage usage information
     */
    static getStorageInfo() {
        try {
            const used = new Blob([localStorage.getItem(StorageManager.STORAGE_KEY) || '']).size;
            return {
                used: used,
                usedMB: (used / 1024 / 1024).toFixed(2)
            };
        } catch {
            return { used: 0, usedMB: '0.00' };
        }
    }
}

// ===== NOTIFICATION SYSTEM =====
class NotificationManager {
    static show(message, type = 'info', duration = 3000) {
        const toast = document.getElementById('toast');
        const icon = toast.querySelector('.toast-icon');
        const messageEl = toast.querySelector('.toast-message');

        // Set icon based on type
        const icons = {
            success: 'fas fa-check-circle',
            error: 'fas fa-exclamation-circle',
            warning: 'fas fa-exclamation-triangle',
            info: 'fas fa-info-circle'
        };

        icon.className = `toast-icon ${icons[type] || icons.info}`;
        messageEl.textContent = message;
        toast.className = `toast ${type}`;

        // Show toast
        toast.classList.add('show');

        // Auto hide
        setTimeout(() => {
            toast.classList.remove('show');
        }, duration);

        Logger.info(`Notification shown: ${type}`, { message });
    }

    static success(message, duration) { NotificationManager.show(message, 'success', duration); }
    static error(message, duration) { NotificationManager.show(message, 'error', duration); }
    static warning(message, duration) { NotificationManager.show(message, 'warning', duration); }
    static info(message, duration) { NotificationManager.show(message, 'info', duration); }
}

// ===== MODAL MANAGER =====
class ModalManager {
    static show(modalId) {
        const modal = document.getElementById(modalId);
        if (modal) {
            modal.classList.add('show');
            modal.setAttribute('aria-hidden', 'false');
            
            // Focus first focusable element
            const focusable = modal.querySelector('input, textarea, button:not([disabled])');
            if (focusable) {
                setTimeout(() => focusable.focus(), 100);
            }
            
            // Trap focus within modal
            ModalManager.trapFocus(modal);
            
            Logger.info(`Modal shown: ${modalId}`);
        }
    }

    static hide(modalId) {
        const modal = document.getElementById(modalId);
        if (modal) {
            modal.classList.remove('show');
            modal.setAttribute('aria-hidden', 'true');
            Logger.info(`Modal hidden: ${modalId}`);
        }
    }

    static trapFocus(modal) {
        const focusableElements = modal.querySelectorAll(
            'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
        );
        
        if (focusableElements.length === 0) return;
        
        const firstElement = focusableElements[0];
        const lastElement = focusableElements[focusableElements.length - 1];

        modal.addEventListener('keydown', (e) => {
            if (e.key === 'Tab') {
                if (e.shiftKey) {
                    if (document.activeElement === firstElement) {
                        e.preventDefault();
                        lastElement.focus();
                    }
                } else {
                    if (document.activeElement === lastElement) {
                        e.preventDefault();
                        firstElement.focus();
                    }
                }
            }
        });
    }
}

// ===== MAIN TASK MANAGER CLASS =====
class TaskManager {
    constructor() {
        this.tasks = [];
        this.currentFilter = 'all';
        this.currentEditingTask = null;
        
        this.initializeElements();
        this.bindEvents();
        this.loadTasks();
        this.checkBrowserCompatibility();
        
        Logger.info('TaskManager initialized successfully');
    }

    /**
     * Initialize DOM element references
     */
    initializeElements() {
        // Input elements
        this.taskInput = document.getElementById('taskInput');
        this.addTaskBtn = document.getElementById('addTaskBtn');
        
        // List and display elements
        this.taskList = document.getElementById('taskList');
        this.emptyState = document.getElementById('emptyState');
        
        // Statistics elements
        this.totalTasksEl = document.getElementById('totalTasks');
        this.pendingTasksEl = document.getElementById('pendingTasks');
        this.completedTasksEl = document.getElementById('completedTasks');
        
        // Filter elements
        this.filterBtns = document.querySelectorAll('.filter-btn');
        
        // Import/Export elements
        this.exportBtn = document.getElementById('exportBtn');
        this.importBtn = document.getElementById('importBtn');
        this.importInput = document.getElementById('importInput');
        
        // Modal elements
        this.editModal = document.getElementById('editModal');
        this.editTaskInput = document.getElementById('editTaskInput');
        this.saveEditBtn = document.getElementById('saveEditBtn');
        this.cancelEditBtn = document.getElementById('cancelEditBtn');
        this.closeModalBtn = document.getElementById('closeModalBtn');
        
        this.confirmModal = document.getElementById('confirmModal');
        this.confirmMessage = document.getElementById('confirmMessage');
        this.confirmActionBtn = document.getElementById('confirmActionBtn');
        this.cancelConfirmBtn = document.getElementById('cancelConfirmBtn');
    }

    /**
     * Bind all event listeners
     */
    bindEvents() {
        // Task input events
        this.addTaskBtn.addEventListener('click', () => this.addTask());
        this.taskInput.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                e.preventDefault();
                this.addTask();
            }
        });

        // Filter events
        this.filterBtns.forEach(btn => {
            btn.addEventListener('click', (e) => {
                this.setFilter(e.target.dataset.filter);
            });
        });

        // Import/Export events
        this.exportBtn.addEventListener('click', () => this.exportTasks());
        this.importBtn.addEventListener('click', () => this.importInput.click());
        this.importInput.addEventListener('change', (e) => this.importTasks(e));

        // Modal events
        this.saveEditBtn.addEventListener('click', () => this.saveEdit());
        this.cancelEditBtn.addEventListener('click', () => this.cancelEdit());
        this.closeModalBtn.addEventListener('click', () => this.cancelEdit());
        
        this.confirmActionBtn.addEventListener('click', () => this.confirmAction());
        this.cancelConfirmBtn.addEventListener('click', () => this.cancelConfirm());

        // Modal overlay events
        this.editModal.querySelector('.modal-overlay').addEventListener('click', () => this.cancelEdit());
        this.confirmModal.querySelector('.modal-overlay').addEventListener('click', () => this.cancelConfirm());

        // Keyboard shortcuts
        document.addEventListener('keydown', (e) => this.handleKeyboardShortcuts(e));

        Logger.info('All event listeners bound successfully');
    }

    /**
     * Handle keyboard shortcuts
     */
    handleKeyboardShortcuts(e) {
        // Escape key to close modals
        if (e.key === 'Escape') {
            if (this.editModal.classList.contains('show')) {
                this.cancelEdit();
            } else if (this.confirmModal.classList.contains('show')) {
                this.cancelConfirm();
            }
        }

        // Ctrl/Cmd + Enter for quick add
        if ((e.ctrlKey || e.metaKey) && e.key === 'Enter') {
            e.preventDefault();
            this.taskInput.focus();
        }
    }

    /**
     * Check browser compatibility
     */
    checkBrowserCompatibility() {
        if (!StorageManager.isAvailable()) {
            NotificationManager.warning(
                'Local storage is not available. Tasks will not persist between sessions.',
                5000
            );
            Logger.warn('localStorage is not available');
        }

        // Check for required APIs
        const requiredAPIs = ['JSON', 'Date', 'Array.prototype.filter'];
        const missing = requiredAPIs.filter(api => {
            try {
                return eval(api) === undefined;
            } catch {
                return true;
            }
        });

        if (missing.length > 0) {
            Logger.warn('Missing browser APIs', missing);
        }
    }

    /**
     * Load tasks from storage
     */
    loadTasks() {
        try {
            this.tasks = StorageManager.loadTasks();
            this.renderTasks();
            this.updateStatistics();
            
            if (this.tasks.length > 0) {
                NotificationManager.success(`Loaded ${this.tasks.length} tasks from storage`);
            }
        } catch (error) {
            Logger.error('Failed to load tasks', error);
            NotificationManager.error('Failed to load saved tasks');
        }
    }

    /**
     * Save tasks to storage
     */
    saveTasks() {
        try {
            StorageManager.saveTasks(this.tasks);
        } catch (error) {
            Logger.error('Failed to save tasks', error);
            NotificationManager.error(error.message);
        }
    }

    /**
     * Add a new task
     */
    addTask() {
        const text = this.taskInput.value;
        const validation = Utils.validateTaskText(text);
        
        if (!validation.valid) {
            NotificationManager.error(validation.error);
            this.taskInput.focus();
            return;
        }

        const task = {
            id: Utils.generateId(),
            text: Utils.sanitizeHtml(validation.text),
            completed: false,
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString()
        };

        this.tasks.unshift(task); // Add to beginning of array
        this.taskInput.value = '';
        this.saveTasks();
        this.renderTasks();
        this.updateStatistics();
        
        NotificationManager.success('Task added successfully');
        Logger.info('Task added', { id: task.id, text: task.text });
    }

    /**
     * Toggle task completion status
     */
    toggleTask(taskId) {
        const task = this.tasks.find(t => t.id === taskId);
        if (!task) {
            Logger.error('Task not found for toggle', { taskId });
            return;
        }

        task.completed = !task.completed;
        task.updatedAt = new Date().toISOString();
        
        this.saveTasks();
        this.renderTasks();
        this.updateStatistics();
        
        const action = task.completed ? 'completed' : 'marked as pending';
        NotificationManager.success(`Task ${action}`);
        Logger.info('Task toggled', { id: task.id, completed: task.completed });
    }

    /**
     * Start editing a task
     */
    editTask(taskId) {
        const task = this.tasks.find(t => t.id === taskId);
        if (!task) {
            Logger.error('Task not found for edit', { taskId });
            return;
        }

        if (task.completed) {
            NotificationManager.warning('Completed tasks cannot be edited');
            return;
        }

        this.currentEditingTask = task;
        this.editTaskInput.value = task.text;
        ModalManager.show('editModal');
        
        Logger.info('Task edit started', { id: task.id });
    }

    /**
     * Save task edit
     */
    saveEdit() {
        if (!this.currentEditingTask) return;

        const text = this.editTaskInput.value;
        const validation = Utils.validateTaskText(text);
        
        if (!validation.valid) {
            NotificationManager.error(validation.error);
            this.editTaskInput.focus();
            return;
        }

        this.currentEditingTask.text = Utils.sanitizeHtml(validation.text);
        this.currentEditingTask.updatedAt = new Date().toISOString();
        
        this.saveTasks();
        this.renderTasks();
        this.updateStatistics();
        
        ModalManager.hide('editModal');
        this.currentEditingTask = null;
        
        NotificationManager.success('Task updated successfully');
        Logger.info('Task updated', { id: this.currentEditingTask?.id });
    }

    /**
     * Cancel task edit
     */
    cancelEdit() {
        ModalManager.hide('editModal');
        this.currentEditingTask = null;
        Logger.info('Task edit cancelled');
    }

    /**
     * Delete a task with confirmation
     */
    deleteTask(taskId) {
        const task = this.tasks.find(t => t.id === taskId);
        if (!task) {
            Logger.error('Task not found for delete', { taskId });
            return;
        }

        this.showConfirmDialog(
            `Are you sure you want to delete this task? This action cannot be undone.`,
            () => {
                this.tasks = this.tasks.filter(t => t.id !== taskId);
                this.saveTasks();
                this.renderTasks();
                this.updateStatistics();
                
                NotificationManager.success('Task deleted successfully');
                Logger.info('Task deleted', { id: taskId });
            }
        );
    }

    /**
     * Show confirmation dialog
     */
    showConfirmDialog(message, onConfirm) {
        this.confirmMessage.textContent = message;
        this.pendingConfirmAction = onConfirm;
        ModalManager.show('confirmModal');
    }

    /**
     * Confirm pending action
     */
    confirmAction() {
        if (this.pendingConfirmAction) {
            this.pendingConfirmAction();
            this.pendingConfirmAction = null;
        }
        ModalManager.hide('confirmModal');
    }

    /**
     * Cancel confirmation
     */
    cancelConfirm() {
        this.pendingConfirmAction = null;
        ModalManager.hide('confirmModal');
    }

    /**
     * Set current filter
     */
    setFilter(filter) {
        this.currentFilter = filter;
        
        // Update filter button states
        this.filterBtns.forEach(btn => {
            btn.classList.toggle('active', btn.dataset.filter === filter);
        });
        
        this.renderTasks();
        Logger.info('Filter changed', { filter });
    }

    /**
     * Get filtered tasks based on current filter
     */
    getFilteredTasks() {
        switch (this.currentFilter) {
            case 'pending':
                return this.tasks.filter(task => !task.completed);
            case 'completed':
                return this.tasks.filter(task => task.completed);
            default:
                return this.tasks;
        }
    }

    /**
     * Render all tasks
     */
    renderTasks() {
        const filteredTasks = this.getFilteredTasks();
        
        if (filteredTasks.length === 0) {
            this.taskList.style.display = 'none';
            this.emptyState.style.display = 'block';
            return;
        }
        
        this.taskList.style.display = 'block';
        this.emptyState.style.display = 'none';
        
        this.taskList.innerHTML = filteredTasks.map(task => this.createTaskHTML(task)).join('');
        
        // Bind task-specific events
        this.bindTaskEvents();
        
        Logger.debug('Tasks rendered', { count: filteredTasks.length, filter: this.currentFilter });
    }

    /**
     * Create HTML for a single task
     */
    createTaskHTML(task) {
        const createdDate = Utils.formatDate(task.createdAt);
        const updatedDate = task.createdAt !== task.updatedAt ? Utils.formatDate(task.updatedAt) : null;
        
        return `
            <div class="task-item ${task.completed ? 'completed' : ''}" data-task-id="${task.id}" role="listitem">
                <div class="task-checkbox ${task.completed ? 'checked' : ''}" 
                     role="checkbox" 
                     aria-checked="${task.completed}"
                     tabindex="0"
                     aria-label="Mark task as ${task.completed ? 'pending' : 'completed'}">
                </div>
                <div class="task-content">
                    <div class="task-text">${task.text}</div>
                    <div class="task-meta">
                        <span>Created: ${createdDate}</span>
                        ${updatedDate ? `<span>Updated: ${updatedDate}</span>` : ''}
                    </div>
                </div>
                <div class="task-actions">
                    <button class="task-action-btn edit-btn" 
                            aria-label="Edit task"
                            title="Edit Task"
                            ${task.completed ? 'disabled' : ''}>
                        <i class="fas fa-edit" aria-hidden="true"></i>
                    </button>
                    <button class="task-action-btn delete-btn" 
                            aria-label="Delete task"
                            title="Delete Task">
                        <i class="fas fa-trash" aria-hidden="true"></i>
                    </button>
                </div>
            </div>
        `;
    }

    /**
     * Bind events for task items
     */
    bindTaskEvents() {
        // Checkbox events
        this.taskList.querySelectorAll('.task-checkbox').forEach(checkbox => {
            const taskId = checkbox.closest('.task-item').dataset.taskId;
            
            checkbox.addEventListener('click', () => this.toggleTask(taskId));
            checkbox.addEventListener('keypress', (e) => {
                if (e.key === 'Enter' || e.key === ' ') {
                    e.preventDefault();
                    this.toggleTask(taskId);
                }
            });
        });

        // Edit button events
        this.taskList.querySelectorAll('.edit-btn').forEach(btn => {
            if (!btn.disabled) {
                const taskId = btn.closest('.task-item').dataset.taskId;
                btn.addEventListener('click', () => this.editTask(taskId));
            }
        });

        // Delete button events
        this.taskList.querySelectorAll('.delete-btn').forEach(btn => {
            const taskId = btn.closest('.task-item').dataset.taskId;
            btn.addEventListener('click', () => this.deleteTask(taskId));
        });
    }

    /**
     * Update statistics display
     */
    updateStatistics() {
        const total = this.tasks.length;
        const completed = this.tasks.filter(task => task.completed).length;
        const pending = total - completed;

        this.totalTasksEl.textContent = total;
        this.completedTasksEl.textContent = completed;
        this.pendingTasksEl.textContent = pending;
        
        Logger.debug('Statistics updated', { total, completed, pending });
    }

    /**
     * Export tasks to JSON file
     */
    exportTasks() {
        try {
            const exportData = {
                version: StorageManager.STORAGE_VERSION,
                exportDate: new Date().toISOString(),
                taskCount: this.tasks.length,
                statistics: {
                    total: this.tasks.length,
                    completed: this.tasks.filter(t => t.completed).length,
                    pending: this.tasks.filter(t => !t.completed).length
                },
                tasks: this.tasks
            };

            const blob = new Blob([JSON.stringify(exportData, null, 2)], { type: 'application/json' });
            const url = URL.createObjectURL(blob);
            
            const a = document.createElement('a');
            a.href = url;
            a.download = `tasks_export_${new Date().toISOString().split('T')[0]}.json`;
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            
            URL.revokeObjectURL(url);
            
            NotificationManager.success(`Exported ${this.tasks.length} tasks successfully`);
            Logger.info('Tasks exported', { count: this.tasks.length });
        } catch (error) {
            Logger.error('Export failed', error);
            NotificationManager.error('Failed to export tasks');
        }
    }

    /**
     * Import tasks from JSON file
     */
    importTasks(event) {
        const file = event.target.files[0];
        if (!file) return;

        const reader = new FileReader();
        reader.onload = (e) => {
            try {
                const data = JSON.parse(e.target.result);
                
                // Validate import data
                if (!data.tasks || !Array.isArray(data.tasks)) {
                    throw new Error('Invalid file format: tasks array not found');
                }

                // Validate and filter imported tasks
                const validTasks = data.tasks.filter(task => {
                    const isValid = StorageManager.validateTask(task);
                    if (!isValid) {
                        Logger.warn('Invalid task in import file', task);
                    }
                    return isValid;
                });

                if (validTasks.length === 0) {
                    NotificationManager.warning('No valid tasks found in the import file');
                    return;
                }

                // Add imported tasks to existing tasks (avoiding duplicates by ID)
                const existingIds = new Set(this.tasks.map(t => t.id));
                const newTasks = validTasks.filter(task => !existingIds.has(task.id));
                
                this.tasks = [...newTasks, ...this.tasks];
                this.saveTasks();
                this.renderTasks();
                this.updateStatistics();
                
                const skipped = validTasks.length - newTasks.length;
                let message = `Imported ${newTasks.length} tasks successfully`;
                if (skipped > 0) {
                    message += ` (${skipped} duplicates skipped)`;
                }
                
                NotificationManager.success(message);
                Logger.info('Tasks imported', { 
                    imported: newTasks.length, 
                    skipped, 
                    total: validTasks.length 
                });
                
            } catch (error) {
                Logger.error('Import failed', error);
                NotificationManager.error(`Import failed: ${error.message}`);
            }
        };
        
        reader.onerror = () => {
            Logger.error('File read error');
            NotificationManager.error('Failed to read the import file');
        };
        
        reader.readAsText(file);
        
        // Reset file input
        event.target.value = '';
    }

    /**
     * Get application statistics for debugging
     */
    getStats() {
        return {
            tasks: {
                total: this.tasks.length,
                completed: this.tasks.filter(t => t.completed).length,
                pending: this.tasks.filter(t => !t.completed).length
            },
            storage: StorageManager.getStorageInfo(),
            filter: this.currentFilter,
            version: StorageManager.STORAGE_VERSION
        };
    }
}

// ===== APPLICATION INITIALIZATION =====
document.addEventListener('DOMContentLoaded', () => {
    try {
        // Initialize the task manager
        window.taskManager = new TaskManager();
        
        // Make utilities available for debugging
        window.TaskManagerUtils = {
            Logger,
            Utils,
            StorageManager,
            NotificationManager,
            ModalManager
        };
        
        Logger.info('Task Manager Application started successfully');
        
        // Show welcome message for first-time users
        if (window.taskManager.tasks.length === 0) {
            setTimeout(() => {
                NotificationManager.info('Welcome to Task Manager! Add your first task to get started.');
            }, 1000);
        }
        
    } catch (error) {
        Logger.error('Failed to initialize Task Manager', error);
        console.error('Task Manager initialization failed:', error);
        
        // Show error to user
        document.body.innerHTML = `
            <div style="text-align: center; padding: 2rem; font-family: Arial, sans-serif;">
                <h1 style="color: #dc3545;">Application Error</h1>
                <p>Failed to initialize Task Manager. Please refresh the page and try again.</p>
                <p style="color: #6c757d; font-size: 0.9rem;">Error: ${error.message}</p>
            </div>
        `;
    }
});

// ===== ERROR HANDLING =====
window.addEventListener('error', (event) => {
    Logger.error('Global error caught', {
        message: event.message,
        filename: event.filename,
        lineno: event.lineno,
        colno: event.colno,
        error: event.error
    });
});

window.addEventListener('unhandledrejection', (event) => {
    Logger.error('Unhandled promise rejection', event.reason);
    event.preventDefault();
});