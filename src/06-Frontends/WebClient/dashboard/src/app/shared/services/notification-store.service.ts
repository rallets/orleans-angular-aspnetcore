import { Injectable } from '@angular/core';
import { NotificationsService, NotificationType } from 'angular2-notifications';
import { BehaviorSubject } from 'rxjs';
import { Notification, NotificationLevel } from '../models/notification.model';

@Injectable({
  providedIn: 'root'
})
export class NotificationStoreService {

  private _notifications: BehaviorSubject<Notification[]> = new BehaviorSubject([]);

  private notificationType: Map<NotificationLevel, NotificationType> = new Map<NotificationLevel, NotificationType>([
    [NotificationLevel.error, NotificationType.Error],
    [NotificationLevel.info, NotificationType.Info],
    [NotificationLevel.success, NotificationType.Success],
    [NotificationLevel.warning, NotificationType.Warn],
  ]);

  constructor(
    private notificationsService: NotificationsService
  ) { }

  get notifications$() {
    return this._notifications.asObservable();
  }

  private _dispatch(notification: Notification) {
    this._notifications.next([...this._notifications.getValue(), notification]);
    this.showNotification(notification);
  }

  dispatch(level: NotificationLevel, message: string) {
    const notification = new Notification(level, message);
    this._dispatch(notification);
  }

  dismiss(notification: Notification) {
    const items = this._notifications.getValue().filter(x => x.id !== notification.id);
    this._notifications.next(items);
  }

  dismissAll() {
    this._notifications.next([]);
  }

  private showNotification(notification: Notification) {
    const type = this.notificationType.get(notification.level);

    const toast = this.notificationsService.create('', notification.message, type, {
      id: notification.id,
    });

  }
}
