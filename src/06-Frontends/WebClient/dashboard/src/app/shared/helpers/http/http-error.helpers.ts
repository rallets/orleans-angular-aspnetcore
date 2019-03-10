import { HttpErrorResponse } from '@angular/common/http';
import { HttpModelStateDictionary } from '../../models/http-model-state-dictionary.model';
import { NotificationStoreService } from '../../services/notification-store.service';
import { throwError } from 'rxjs';
import { NotificationLevel } from '../../models/notification.model';

export function handleHttpError(error: HttpErrorResponse, notificationService: NotificationStoreService) {
  const type = GetErrorStatusDescription(error.status);
  let message = 'An error occurred';

  try {
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred
      message = error.error.message || '';
    } else if (error.error instanceof Object) {
      if (error.status !== 0 && error.error) {
        // Try to parse a ASP.NET Core Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary
        const e = error.error as HttpModelStateDictionary;
        const errors = Object.entries(e);

        if (errors && errors.length > 0) {
          message = GetStateMessage(errors);
        }
      }
    } else {
      // The backend returned a generic error.
      if (error.error && !isHTML(error.error)) {
        message = error.error;
      }
    }
    message = `${type}<br/>${message}`;
  } catch (e) {
    console.log(e);
  }

  // message = `${message}<br/>Please contact us if the error persists.`;

  if (notificationService) {
    notificationService.dispatch(NotificationLevel.error, message);
  } else {
    console.error(message);
  }

  // return an observable with a user-facing error message
  return throwError(Error('An error occurred'));
}

function GetErrorStatusDescription(httpStatusCode: number) {
  let type = 'Server error';
  if (httpStatusCode === 404) {
    type = 'Not found';
  } else if (httpStatusCode === 400) {
    type = 'Bad request';
  } else if (httpStatusCode === 409) {
    type = 'Conflict';
  } else if (httpStatusCode === 403) {
    type = 'Forbidden';
  }
  return type;
}

function GetStateMessage(errors: [string, string[]][]) {
  let message = '';
  for (const [key, value] of errors) {
    const messages = value instanceof Array ? value : [value];

    if (message) {
      message = `${message} <br/> `;
    }
    if (key) {
      message = `${message} ${key}: ${messages.join(', ')}`;
    } else {
      message = `${message} ${messages.join(', ')}`;
    }
  }
  return message;
}

function isHTML(stringContent: string) {
  const doc = new DOMParser().parseFromString(stringContent, 'text/html');
  return Array.from(doc.body.childNodes).some(node => node.nodeType === 1);
}
